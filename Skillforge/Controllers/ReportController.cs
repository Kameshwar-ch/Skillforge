using Cronos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Constants;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Service;
using System.Security.Claims;

namespace Skillforge.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = nameof(UserRole.Admin))]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Creates a recurring report schedule using a cron expression.
    /// The background service will automatically run the report at each scheduled interval
    /// and send a notification on completion.
    /// </summary>
    /// <param name="dto">Scope and cron expression for the schedule.</param>
    /// <returns>201 Created with schedule details including the next run time.</returns>
    
    [HttpPost("schedule")]
    [ProducesResponseType(typeof(ReportScheduleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateReportScheduleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var adminIdClaim = User.FindFirstValue("id");
            if (!int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { message = ReportErrorMessages.InvalidTokenClaims });

            var result = await _reportService.CreateScheduleAsync(dto, adminId);
            return StatusCode(201, result);
        }
        catch (CronFormatException)
        {
            return BadRequest(new { message = ReportErrorMessages.InvalidCronExpression });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Returns all report schedules.
    /// </summary>
    [HttpGet("schedules")]
    [ProducesResponseType(typeof(IEnumerable<ReportScheduleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSchedules()
    {
        var result = await _reportService.GetAllSchedulesAsync();
        return Ok(result);
    }
}
