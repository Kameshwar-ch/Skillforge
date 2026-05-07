using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly SkillForgeDB         _context;

    private const string CourseAccessedAction   = "CourseAccessed";
    private const string AttendanceMarkedAction = "AttendanceMarked";

    public AttendanceService(IAttendanceRepository attendanceRepository, SkillForgeDB context)
    {
        _attendanceRepository = attendanceRepository;
        _context              = context;
    }

    /// <summary>
    /// Marks attendance for single or bulk enrollments in a course.
    /// Single → send one record with EnrollmentID and Status.
    /// Bulk   → send multiple records, system checks AuditLog per employee.
    /// Upserts each record and logs trainer action in AuditLog.
    /// </summary>
    public async Task<AttendanceResponseDto> MarkAttendanceAsync(MarkAttendanceDto dto, int trainerID)
    {
        if (dto.CourseID <= 0)
            throw new InvalidOperationException("Invalid CourseID.");

        if (dto.AttendanceDate == default)
            throw new InvalidOperationException("AttendanceDate is required.");

        if (dto.AttendanceDate.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException($"Invalid date. {dto.AttendanceDate:yyyy-MM-dd} is a future date.");

        if (dto.Records == null || !dto.Records.Any())
            throw new InvalidOperationException("At least one attendance record is required.");

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseID == dto.CourseID);

        if (course == null)
            throw new KeyNotFoundException($"Course {dto.CourseID} not found.");

        if (course.TrainerID != trainerID)
            throw new UnauthorizedAccessException("You are not authorized to mark attendance for this course.");

        // Batch fetch AuditLog CourseAccessed for all enrollments on this date
        var enrollmentIDs    = dto.Records.Select(r => r.EnrollmentID).ToList();
        var enrollmentList   = await _context.Enrollments
            .Include(e => e.EmployeeIdNavigation)
            .Where(e => enrollmentIDs.Contains(e.EnrollmentID))
            .ToListAsync();

        var employeeIDList   = enrollmentList.Select(e => e.EmployeeID).ToList();

        var rawLogs = await _context.AuditLogs
            .Where(a =>
                a.UserID    != null                           &&
                employeeIDList.Contains(a.UserID.Value)       &&
                a.Action    == CourseAccessedAction            &&
                a.Resource  == $"Course/{dto.CourseID}"       &&
                a.Timestamp >= dto.AttendanceDate.Date         &&
                a.Timestamp <  dto.AttendanceDate.Date.AddDays(1))
            .Select(a => new { UserID = a.UserID!.Value, a.Timestamp })
            .ToListAsync();

        var firstAccessPerEmployee = rawLogs
            .GroupBy(a => a.UserID)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(a => a.Timestamp).First().Timestamp
            );

        var records = new List<AttendanceRecordResultDto>();

        foreach (var record in dto.Records)
        {
            if (record.EnrollmentID <= 0)
                throw new InvalidOperationException("Invalid EnrollmentID.");

            var enrollment = enrollmentList
                .FirstOrDefault(e => e.EnrollmentID == record.EnrollmentID);

            if (enrollment == null)
                throw new KeyNotFoundException($"Enrollment {record.EnrollmentID} not found.");

            if (enrollment.CourseID != dto.CourseID)
                throw new InvalidOperationException($"Enrollment {record.EnrollmentID} does not belong to Course {dto.CourseID}.");

            if (enrollment.Status)
                throw new InvalidOperationException($"Cannot mark attendance. Enrollment {record.EnrollmentID} is not in progress.");

            var isAccessed = firstAccessPerEmployee.ContainsKey(enrollment.EmployeeID);

            var attendance = new Attendance
            {
                EnrollmentID   = record.EnrollmentID,
                AttendanceDate = isAccessed
                                 ? firstAccessPerEmployee[enrollment.EmployeeID]
                                 : dto.AttendanceDate.Date,
                Status         = record.Status
            };

            await _attendanceRepository.UpsertAttendanceAsync(attendance);

            records.Add(new AttendanceRecordResultDto
            {
                EnrollmentID = enrollment.EnrollmentID,
                EmployeeName = enrollment.EmployeeIdNavigation.Name,
                Status       = record.Status.ToString()
            });
        }

        _context.AuditLogs.Add(new AuditLog
        {
            UserID    = trainerID,
            Action    = AttendanceMarkedAction,
            Resource  = $"Course/{dto.CourseID}",
            Timestamp = DateTime.Now
        });
        await _context.SaveChangesAsync();

        var presentCount = records.Count(r => r.Status == "Present");
        var absentCount  = records.Count(r => r.Status == "Absent");

        return new AttendanceResponseDto
        {
            CourseID       = dto.CourseID,
            AttendanceDate = dto.AttendanceDate,
            TotalMarked    = records.Count,
            PresentCount   = presentCount,
            AbsentCount    = absentCount,
            Records        = records,
            Message        = "Attendance marked successfully."
        };
    }

    /// <summary>
    /// Returns all active enrollments with CourseStatus and LoginDate for a course on a given date.
    /// </summary>
    public async Task<GetCourseAttendanceResponseDto> GetCourseAttendanceAsync(int courseID, DateTime date, int trainerID)
    {
        if (courseID <= 0)
            throw new InvalidOperationException("Invalid CourseID.");

        if (date == default)
            throw new InvalidOperationException("Date is required.");

        if (date.Date > DateTime.UtcNow.Date)
            throw new InvalidOperationException($"Invalid date. {date:yyyy-MM-dd} is a future date.");

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseID == courseID);

        if (course == null)
            throw new KeyNotFoundException($"Course {courseID} not found.");

        if (course.TrainerID != trainerID)
            throw new UnauthorizedAccessException("You are not authorized to access this course.");

        var enrollments = await _context.Enrollments
            .Include(e => e.EmployeeIdNavigation)
            .Where(e => e.CourseID == courseID && e.Status == false)
            .ToListAsync();

        if (!enrollments.Any())
            throw new KeyNotFoundException("No active enrollments found for this course.");

        var enrolledEmployeeIDList = enrollments
            .Select(e => e.EmployeeID)
            .ToList();

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

        var firstAccessPerEmployee = rawLogs
            .GroupBy(a => a.UserID)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(a => a.Timestamp).First().Timestamp
            );

        var records = enrollments.Select(e => new CourseAttendanceDto
        {
            EnrollmentID = e.EnrollmentID,
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