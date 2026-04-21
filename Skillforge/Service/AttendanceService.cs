using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly SkillForgeDB _context;

    // AuditLog constants
    private const string CourseAccessedAction   = "CourseAccessed";
    private const string AttendanceMarkedAction = "AttendanceMarked";

    /// <summary>
    /// Initializes AttendanceService with repository and database context.
    /// </summary>
    /// <param name="attendanceRepository">Repository for upsert operations.</param>
    /// <param name="context">Database context for EF Core queries.</param>
    public AttendanceService(IAttendanceRepository attendanceRepository, SkillForgeDB context)
    {
        _attendanceRepository = attendanceRepository;
        _context = context;
    }

    // Single Attendance 

    /// <summary>
    /// Marks attendance for a single enrollment.
    /// Validates EnrollmentID, date and enrollment status.
    /// Checks AuditLog CourseAccessed for that employee on that date.
    /// Present if accessed, Absent if not. Upserts attendance record.
    /// Logs trainer action in AuditLog.
    /// </summary>
    /// <param name="dto">Contains EnrollmentID and AttendanceDate.</param>
    /// <param name="trainerID">ID of the trainer extracted from JWT.</param>
    /// <returns>AttendanceID and success/update message.</returns>
    public async Task<AttendanceResponseDto> MarkAttendanceAsync(MarkAttendanceDto dto, int trainerID)
    {
        // Validate EnrollmentID
        if (dto.EnrollmentID <= 0)
            throw new InvalidOperationException("Invalid EnrollmentID.");

        // Validate AttendanceDate is not empty
        if (dto.AttendanceDate == default)
            throw new InvalidOperationException("AttendanceDate is required.");

        // Validate date is not future
        if (dto.AttendanceDate.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException($"Invalid date. {dto.AttendanceDate:yyyy-MM-dd} is a future date.");

        // Check enrollment exists
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.EnrollmentID == dto.EnrollmentID);

        if (enrollment == null)
            throw new KeyNotFoundException($"Enrollment {dto.EnrollmentID} not found.");

        // Check enrollment is active
        if (enrollment.Status)
            throw new InvalidOperationException("Cannot mark attendance. Enrollment is not in progress.");

       
        // Present if accessed this course, Absent if not
        var attendance = new Attendance
        {
            EnrollmentID   = dto.EnrollmentID,
            AttendanceDate = dto.AttendanceDate,
            Status         = dto.Status
        };

        // Upsert — insert new or update existing for same enrollment + date
        var (result, isNew) = await _attendanceRepository.UpsertAttendanceAsync(attendance);

       
        _context.AuditLogs.Add(new AuditLog
        {
            UserID    = trainerID,
            Action    = AttendanceMarkedAction,
            Resource  = $"Enrollment/{dto.EnrollmentID}",
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();

        return new AttendanceResponseDto
        {
            AttendanceID = result.AttendanceID,
            Message      = isNew
                ? "Attendance marked successfully."
                : "Attendance already marked. Record updated."
        };
    }

    // Bulk Attendance 

    /// <summary>
    /// Marks attendance for all active enrollments in a course in one shot.
    /// Validates CourseID, date and course ownership.
    /// Batch fetches AuditLog CourseAccessed for all employees on that date.
    /// Present if accessed, Absent if not. Upserts all records.
    /// Logs trainer bulk action in AuditLog.
    /// </summary>
    /// <param name="dto">Contains CourseID and AttendanceDate.</param>
    /// <param name="trainerID">ID of the trainer extracted from JWT.</param>
    /// <returns>Summary with TotalMarked, PresentCount, AbsentCount and per-employee records.</returns>
    public async Task<BulkAttendanceResponseDto> BulkMarkAttendanceAsync(BulkMarkAttendanceDto dto, int trainerID)
    {
        // Validate CourseID
        if (dto.CourseID <= 0)
            throw new InvalidOperationException("Invalid CourseID.");

        // Validate AttendanceDate
        if (dto.AttendanceDate == default)
            throw new InvalidOperationException("AttendanceDate is required.");

        // Validate date is not future
        if (dto.AttendanceDate.Date > DateTime.Now.Date)
            throw new InvalidOperationException($"Invalid date. {dto.AttendanceDate:yyyy-MM-dd} is a future date.");

        // Check course exists
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseID == dto.CourseID);

        if (course == null)
            throw new KeyNotFoundException($"Course {dto.CourseID} not found.");

        // Check course belongs to this trainer
        if (course.TrainerID != trainerID)
            throw new UnauthorizedAccessException("You are not authorized to mark attendance for this course.");

        // Get all active enrollments for this course
        var enrollments = await _context.Enrollments
            .Include(e => e.EmployeeIdNavigation)
            .Where(e => e.CourseID == dto.CourseID && e.Status == false)
            .ToListAsync();

        if (!enrollments.Any())
            throw new KeyNotFoundException("No active enrollments found for this course.");

        // Batch fetch CourseAccessed logs for all employees on this date
        var enrolledEmployeeIDList = enrollments
            .Select(e => e.EmployeeID)
            .ToList();

        var rawLogs = await _context.AuditLogs
            .Where(a =>
                a.UserID    != null                              &&
                enrolledEmployeeIDList.Contains(a.UserID.Value)  &&
                a.Action    == CourseAccessedAction               &&
                a.Resource  == $"Course/{dto.CourseID}"          &&
                a.Timestamp >= dto.AttendanceDate.Date            &&
                a.Timestamp <  dto.AttendanceDate.Date.AddDays(1))
            .Select(a => new { UserID = a.UserID!.Value, a.Timestamp })
            .ToListAsync();

        // Group in memory → employees who accessed course that day
        var accessedEmployeeIDs = rawLogs
            .Select(a => a.UserID)
            .ToHashSet();

        var firstAccessPerEmployee = rawLogs
            .GroupBy(a => a.UserID)
            .ToDictionary(
                  g => g.Key,
                  g => g.OrderBy(a => a.Timestamp).First().Timestamp
                 );

        // Mark attendance for all enrollments in one go
        var records = new List<BulkAttendanceRecordDto>();

        foreach (var enrollment in enrollments)
        {
            var isPresent = accessedEmployeeIDs.Contains(enrollment.EmployeeID);

            var attendance = new Attendance
            {
                EnrollmentID   = enrollment.EnrollmentID,
                AttendanceDate = isPresent
                     ? firstAccessPerEmployee[enrollment.EmployeeID]  // AuditLog time
                     : dto.AttendanceDate.Date,
                Status         = isPresent
                                 ? AttendanceStatus.Present
                                 : AttendanceStatus.Absent
            };

            // Upsert each enrollment
            await _attendanceRepository.UpsertAttendanceAsync(attendance);

            records.Add(new BulkAttendanceRecordDto
            {
                EnrollmentID = enrollment.EnrollmentID,
                EmployeeID   = enrollment.EmployeeID,
                EmployeeName = enrollment.EmployeeIdNavigation.Name,
                Status       = isPresent ? "Present" : "Absent"
            });
        }

        // Log trainer bulk action in AuditLog
        _context.AuditLogs.Add(new AuditLog
        {
            UserID    = trainerID,
            Action    = AttendanceMarkedAction,
            Resource  = $"Course/{dto.CourseID}/Bulk",
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();

        var presentCount = records.Count(r => r.Status == "Present");
        var absentCount  = records.Count(r => r.Status == "Absent");

        return new BulkAttendanceResponseDto
        {
            CourseID       = dto.CourseID,
            AttendanceDate = dto.AttendanceDate,
            TotalMarked    = records.Count,
            PresentCount   = presentCount,
            AbsentCount    = absentCount,
            Records        = records,
            Message        = $"Bulk attendance marked successfully. Present: {presentCount}, Absent: {absentCount}."
        };
    }

    //GET Course Attendance

    /// <summary>
    /// Retrieves attendance preview for all active enrollments in a course on a specific date.
    /// Batch fetches AuditLog CourseAccessed for all enrolled employees.
    /// Returns CourseStatus (Accessed/Not Accessed) and LoginDate per employee.
    /// LoginDate = exact AuditLog timestamp if accessed, date with 00:00:00 if not.
    /// </summary>
    /// <param name="courseID">ID of the course.</param>
    /// <param name="date">Date to retrieve attendance for.</param>
    /// <param name="trainerID">ID of the trainer extracted from JWT.</param>
    /// <returns>List of employees with CourseStatus and LoginDate.</returns>
    public async Task<GetCourseAttendanceResponseDto> GetCourseAttendanceAsync(int courseID, DateTime date, int trainerID)
    {
        // Validate CourseID
        if (courseID <= 0)
            throw new InvalidOperationException("Invalid CourseID.");

        // Validate date not missing
        if (date == default)
            throw new InvalidOperationException("Date is required.");

        // Validate date is not future
        if (date.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException($"Invalid date. {date:yyyy-MM-dd} is a future date.");

        // Check course exists
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseID == courseID);

        if (course == null)
            throw new KeyNotFoundException($"Course {courseID} not found.");

        // Check course belongs to this trainer
        if (course.TrainerID != trainerID)
            throw new UnauthorizedAccessException("You are not authorized to access this course.");

        // Get all active enrollments for this course
        var enrollments = await _context.Enrollments
            .Include(e => e.EmployeeIdNavigation)
            .Where(e => e.CourseID == courseID && e.Status == false)
            .ToListAsync();

        if (!enrollments.Any())
            throw new KeyNotFoundException("No active enrollments found for this course.");

        // Use List — EF Core translates List.Contains() to SQL IN clause 
        var enrolledEmployeeIDList = enrollments
            .Select(e => e.EmployeeID)
            .ToList();

        // Batch fetch all CourseAccessed logs for this course on this date
        // UserID is int? (nullable) — check null before comparing
        var rawLogs = await _context.AuditLogs
            .Where(a =>
                a.UserID    != null                              &&
                enrolledEmployeeIDList.Contains(a.UserID.Value)  &&
                a.Action    == CourseAccessedAction               &&
                a.Resource  == $"Course/{courseID}"              &&
                a.Timestamp >= date.Date                         &&
                a.Timestamp <  date.Date.AddDays(1))
            .Select(a => new { UserID = a.UserID!.Value, a.Timestamp })
            .ToListAsync();

        // Group in memory → first access time per employee that day
        var firstAccessPerEmployee = rawLogs
            .GroupBy(a => a.UserID)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(a => a.Timestamp).First().Timestamp
            );

        // Build records
        // Accessed     → LoginDate = exact AuditLog timestamp e.g. 2026-04-20T09:46:58
        // Not Accessed → LoginDate = date with 00:00:00      e.g. 2026-04-20T00:00:00
        var records = enrollments.Select(e => new CourseAttendanceDto
        {
            EnrollmentID = e.EnrollmentID,
            EmployeeID   = e.EmployeeID,
            EmployeeName = e.EmployeeIdNavigation.Name,
            CourseStatus = firstAccessPerEmployee.ContainsKey(e.EmployeeID)
                           ? "Accessed" : "Not Accessed",
            LoginDate    = firstAccessPerEmployee.TryGetValue(e.EmployeeID, out var t)
                           ? t
                           : date.Date
        }).ToList();

        return new GetCourseAttendanceResponseDto
        {
            CourseID       = courseID,
            AttendanceDate = date,
            Records        = records
        };
    }
}