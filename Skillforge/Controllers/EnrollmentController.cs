using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
using Skillforge.Service;
using System.Security.Claims;

namespace Skillforge.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class EnrollmentController : ControllerBase
{
    IEnrollmentService enrollmentService;
    public EnrollmentController(IEnrollmentService _enrollmentService)
    {
        enrollmentService = _enrollmentService;
    }
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Employee))]
    public async Task<IActionResult> Enroll(EnrollmentDto dto)
    {
        try
        {
            var id = await enrollmentService.EnrollAsync(dto.CourseId, dto.EmployeeId);
            return Created("", new { enrollmentId = id });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // POST api/v1/Enrollment/bulk - Manager assigns training to multiple employees
    // Returns 201 if all succeed, 200 for partial success, 400 if all fail
    [HttpPost("bulk")]
    [Authorize(Roles = nameof(UserRole.Manager))]
    public async Task<IActionResult> BulkEnroll(BulkEnrollmentRequestDto dto)
    {
        try
        {
            // Extract manager ID from JWT token
            var userIdClaim = User.FindFirstValue("id");
            if (!int.TryParse(userIdClaim, out int managerId))
                return Unauthorized(new { message = "Invalid token." });

            var result = await enrollmentService.BulkEnrollAsync(dto, managerId);

            // All failed
            if (result.Succeeded == 0)
                return BadRequest(result);

            // Partial success
            if (result.Failed > 0)
                return Ok(result);

            // All succeeded
            return StatusCode(201, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

