using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Service;

namespace Skillforge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    /// <summary>
    /// Initializes AttendanceController with the attendance service.
    /// </summary>
    /// <param name="attendanceService">Service for handling attendance operations.</param>
    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // Single Attendance 

    /// <summary>
    /// Marks attendance for a single enrollment.
    /// Checks AuditLog for CourseAccessed on the given date.
    /// Present if employee accessed the course, Absent if not.
    /// Trainer action is logged in AuditLog.
    /// </summary>
    /// <param name="dto">Contains EnrollmentID and AttendanceDate.</param>
    /// <returns>AttendanceID and message indicating success or update.</returns>
    // POST /api/attendance
    [HttpPost]
    [Authorize(Roles = "Trainer")]
    public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid request data." });

        try
        {
            // Extract TrainerID from JWT — saved in AuditLog
            var claim = User.FindFirst("id");
            if (claim == null)
                return Unauthorized(new { message = "Trainer ID not found in token." });

            int trainerID = int.Parse(claim.Value);

            var result = await _attendanceService.MarkAttendanceAsync(dto, trainerID);

            if (result.Message == "Attendance marked successfully.")
                return StatusCode(201, result);
            else
                return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    //  Bulk Attendance 

    /// <summary>
    /// Marks attendance for all active enrollments in a course at once.
    /// Checks AuditLog CourseAccessed for each employee on the given date.
    /// Present if accessed, Absent if not. Trainer bulk action logged in AuditLog.
    /// </summary>
    /// <param name="dto">Contains CourseID and AttendanceDate.</param>
    /// <returns>Summary with TotalMarked, PresentCount, AbsentCount and per-employee records.</returns>
    // POST /api/attendance/bulk
    [HttpPost("bulk")]
    [Authorize(Roles = "Trainer")]
    public async Task<IActionResult> BulkMarkAttendance([FromBody] BulkMarkAttendanceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid request data." });

        try
        {
            var claim = User.FindFirst("id");
            if (claim == null)
                return Unauthorized(new { message = "Trainer ID not found in token." });

            int trainerID = int.Parse(claim.Value);

            var result = await _attendanceService.BulkMarkAttendanceAsync(dto, trainerID);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    //  GET Course Attendance

    /// <summary>
    /// Retrieves attendance preview for all active enrollments in a course on a specific date.
    /// Shows CourseStatus (Accessed/Not Accessed) and LoginDate from AuditLog per employee.
    /// Trainer uses this to review before marking attendance.
    /// </summary>
    /// <param name="courseID">ID of the course.</param>
    /// <param name="date">Date to retrieve attendance for.</param>
    /// <returns>List of employees with CourseStatus and LoginDate.</returns>
    // GET /api/attendance/course/{courseID}?date=2026-04-20
    [HttpGet("course/{courseID}")]
    [Authorize(Roles = "Trainer")]
    public async Task<IActionResult> GetCourseAttendance(int courseID, [FromQuery] DateTime date)
    {
        try
        {
            // Get TrainerID from JWT token
            var claim = User.FindFirst("id");
            if (claim == null)
                return Unauthorized(new { message = "Trainer ID not found in token." });

            int trainerID = int.Parse(claim.Value);

            var result = await _attendanceService.GetCourseAttendanceAsync(courseID, date, trainerID);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
        }
    }
}