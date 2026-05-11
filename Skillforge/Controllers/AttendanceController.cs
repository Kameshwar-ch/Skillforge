using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Domain;
using Skillforge.Service;

namespace Skillforge.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
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

    /// <summary>
    /// Marks attendance for single or bulk enrollments in a course.
    /// Send one record for single, multiple records for bulk.
    /// </summary>
    [HttpPost("Mark-Attendance")]
    [Authorize(Roles = nameof(UserRole.Trainer))]
    public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { message = "Invalid request data." });

        try
        {
            var claim = User.FindFirst("id");
            if (claim == null)
                return Unauthorized(new { message = "Unauthorize user." });

            int trainerID = int.Parse(claim.Value);

            var result = await _attendanceService.MarkAttendanceAsync(dto, trainerID);
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
    [Authorize(Roles = nameof(UserRole.Trainer))]
    public async Task<IActionResult> GetCourseAttendance(int courseID, [FromQuery] DateTime date)
    {
        try
        {
            var claim = User.FindFirst("id");
            if (claim == null)
                return Unauthorized(new { message = "UnAuthorize User.." });

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