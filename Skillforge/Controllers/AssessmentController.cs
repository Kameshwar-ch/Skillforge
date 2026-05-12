using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Service;
using System.Security.Claims;

namespace Skillforge.Controller;

/// <summary>
/// AssessmentController manages course assessment lifecycle.
/// It provides endpoints for creating assessments tied to live courses, restricted to Trainers.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class AssessmentController : ControllerBase
{
    private readonly IAssessmentService _assessmentService;
    private readonly IAuditService _auditService;

    public AssessmentController(IAssessmentService assessmentService, IAuditService auditService)
    {
        _assessmentService = assessmentService;
        _auditService = auditService;
    }

    /// <summary>
    /// Creates a new assessment (Quiz, Exam, or Practical) for a specified live course.
    /// Validates that MaxScore is between 1 and 100 and that the target course is currently live.
    /// On success, logs an audit entry and returns the generated assessmentId.
    /// </summary>
    /// <param name="request">The assessment creation request containing CourseId, Type, and MaxScore.</param>
    /// <returns>
    /// 201 Created with the new assessmentId on success,
    /// 400 Bad Request if validation fails or the course is not live,
    /// or 500 Internal Server Error on unexpected failure.
    /// </returns>
    [HttpPost("save-assessments")]
    [Authorize(Roles = nameof(UserRole.Trainer))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAssessment([FromBody] CreateAssessmentRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var trainerIdClaim = User.FindFirstValue("id");
            int.TryParse(trainerIdClaim, out int trainerId);

            var (success, errorMessage, assessmentId) = await _assessmentService.CreateAssessmentAsync(request);

            if (!success)
                return NotFound(new { message = errorMessage });

            await _auditService.LogAsync(trainerId, "AssessmentCreated", $"Assessment/{assessmentId}");

            return StatusCode(201, new { assessmentId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("update-assessment/{assessmentId}")]
    [Authorize(Roles = nameof(UserRole.Trainer))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAssessment(int assessmentId, [FromBody] UpdateAssessmentRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
       {
            var trainerIdClaim = User.FindFirstValue("id");
            if (!int.TryParse(trainerIdClaim, out int trainerId))
               return Unauthorized();

           var (success, errorMessage) = await _assessmentService.UpdateAssessmentAsync(assessmentId, request);

           if (!success)
                return NotFound(new { message = errorMessage });

            await _auditService.LogAsync(trainerId, "AssessmentUpdated", $"Assessment/{assessmentId}");

            return Ok(new { message = "Assessment updated successfully." });
        }
        catch
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpDelete("delete-assessment/{assessmentId}")]
    [Authorize(Roles = nameof(UserRole.Trainer))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAssessment(int assessmentId)
    {
        try
        {
            var trainerIdClaim = User.FindFirstValue("id");
            if (!int.TryParse(trainerIdClaim, out int trainerId))
                return Unauthorized();

            var (success, errorMessage) = await _assessmentService.DeleteAssessmentAsync(assessmentId);

            if (!success)
                return NotFound(new { message = errorMessage });

            await _auditService.LogAsync(trainerId, "AssessmentDeleted", $"Assessment/{assessmentId}");

            return Ok(new { message = "Assessment deleted successfully." });
        }
        catch
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("get-assessments")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
    public async Task<IActionResult> GetAssessments([FromQuery] AssessmentFilterDto filter)
    {
        try
        {
            var data = await _assessmentService.GetAssessmentsAsync(filter);
            return Ok(data);
        }
        catch
        {
            return StatusCode(500, new { message = "An error occurred" });
        }
    }

    [HttpGet("get-assessment/{assessmentId}")]
    [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
    public async Task<IActionResult> GetAssessmentById(int assessmentId)
    {
        var result = await _assessmentService.GetAssessmentByIdAsync(assessmentId);

        if (result == null)
            return NotFound(new { message = "Assessment not found" });

        return Ok(result);
    }
}
