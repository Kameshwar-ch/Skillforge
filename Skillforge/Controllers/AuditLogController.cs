using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Skillforge.Domain;
using Skillforge.Utility;

namespace Skillforge.Controller;

/// <summary>
/// AuditLogsController manages immutable audit log records.
/// Provides endpoints for retrieving audit logs in a read-only manner,
/// restricted to Admin and HR roles.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.HR)}")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Retrieves a single audit log by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuditLogById(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = AuditLogMessages.InvalidId });

        try
        {
            var log = await _auditLogService.GetAuditLogByIdAsync(id);

            if (log == null)
                return NotFound(new { message = AuditLogMessages.NotFound });

            return Ok(log);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = AuditLogMessages.Error, detail = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves paginated and sortable audit logs.
    /// Default sort is by Timestamp, but can sort by AuditID, UserID, or any property.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "Timestamp",
        [FromQuery] string sortOrder = "desc")
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest(new { message = AuditLogMessages.InvalidPagination });

        try
        {
            var logs = await _auditLogService.GetAuditLogsPagedAsync(page, pageSize, sortBy, sortOrder);

            if (logs.Items == null || !logs.Items.Any())
                return NotFound(new { message = AuditLogMessages.NoLogs });

            return Ok(logs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = AuditLogMessages.Error, detail = ex.Message });
        }
    }
}
