using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Skillforge.Utility;
using Skillforge.Domain;

namespace Skillforge.Controller;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.HR)}")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    private readonly HashSet<string> allowedSortFields = new() { "AuditID", "UserID", "Resource", "Action", "Timestamp" };
    private readonly HashSet<string> allowedSortOrders = new() { "asc", "desc" };

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Retrieves immutable audit logs with optional filters, pagination, and sorting.
    /// </summary>
    /// <param name="auditId">Filter by AuditID (optional)</param>
    /// <param name="userId">Filter by UserID (optional)</param>
    /// <param name="resource">Filter by Resource (e.g. Employee, Payroll)</param>
    /// <param name="action">Filter by Action (e.g. LoginSuccess, LoginDenied)</param>
    /// <param name="timestamp">Filter by Timestamp (yyyy-MM-dd)</param>
    /// <param name="page">Page number (default = 1)</param>
    /// <param name="pageSize">Page size (default = 10)</param>
    /// <param name="sortBy">Sort field (AuditID, UserID, Resource, Action, Timestamp)</param>
    /// <param name="sortOrder">Sort order (asc or desc)</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogResponseDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int? auditId,
        [FromQuery] int? userId,
        [FromQuery] string? resource,
        [FromQuery] string? action,
        [FromQuery] DateTime? timestamp,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        // Validate pagination
        if (page <= 0 || pageSize <= 0)
            return BadRequest(new { message = AuditLogMessages.InvalidPagination });

        //  Default values if null/empty
        if (string.IsNullOrWhiteSpace(sortBy))
            sortBy = "Timestamp";

        if (string.IsNullOrWhiteSpace(sortOrder))
            sortOrder = "desc";

        // Validate sortBy
        if (!allowedSortFields.Contains(sortBy))
            return BadRequest(new { message = AuditLogMessages.InvalidSortBy });

        // Validate sortOrder
        if (!allowedSortOrders.Contains(sortOrder.ToLower()))
            return BadRequest(new { message = AuditLogMessages.InvalidSortOrder });

        try
        {
            var request = new AuditLogFilterRequestDto
            {
                AuditID = auditId,
                UserID = userId,
                Resource = resource,
                Action = action,
                Timestamp = timestamp,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var logs = await _auditLogService.GetAuditLogsFilteredAsync(request);

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
