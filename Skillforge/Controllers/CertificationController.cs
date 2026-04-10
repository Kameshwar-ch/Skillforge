using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Service;
using System.Security.Claims;

namespace Skillforge.Controller;

/// <summary>
/// CertificationController handles issuance of certifications for employees
/// who have successfully passed course assessments.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class CertificationController : ControllerBase
{
    private readonly ICertificationService _certificationService;
    private readonly IAuditService _auditService;

    public CertificationController(ICertificationService certificationService, IAuditService auditService)
    {
        _certificationService = certificationService;
        _auditService = auditService;
    }

    /// <summary>
    /// Issues a certification for an employee who has passed an assessment for the specified course.
    /// Validates that the employee exists, the course is live, and a pass result is on record.
    /// Expiry is automatically set to one year from the issue date.
    /// On success, a notification is triggered and an audit entry is recorded.
    /// </summary>
    /// <param name="request">The certification request containing EmployeeId and CourseId.</param>
    /// <returns>
    /// 201 Created with certification details on success,
    /// 400 Bad Request if validation fails or prerequisites are unmet,
    /// or 500 Internal Server Error on unexpected failure.
    /// </returns>
    [HttpPost("certifications")]
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.HR)}")]
    [ProducesResponseType(typeof(CertificationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IssueCertification([FromBody] IssueCertificationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var issuedByIdClaim = User.FindFirstValue("id");
            int.TryParse(issuedByIdClaim, out int issuedById);

            var (success, errorMessage, certification) = await _certificationService.IssueCertificationAsync(request);

            if (!success)
                return BadRequest(new { message = errorMessage });

            await _auditService.LogAsync(issuedById, "CertificationIssued", $"Certification/{certification!.CertificationId}");

            return StatusCode(201, certification);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
