using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Skillforge.Utility;
using Skillforge.Domain;

namespace Skillforge.Controller
{
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

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
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
            try
            {
                // Pagination validation
                if (page <= 0 || pageSize <= 0)
                    return StatusCode(400, new { message = AuditLogMessages.InvalidPagination });

                // Default sorting if not provided
                if (string.IsNullOrWhiteSpace(sortBy))
                    sortBy = "Timestamp";

                if (string.IsNullOrWhiteSpace(sortOrder))
                    sortOrder = "desc";

                // Validate sortBy
                if (!allowedSortFields.Contains(sortBy))
                    return StatusCode(400, new { message = AuditLogMessages.InvalidSortBy });

                // Validate sortOrder
                if (!allowedSortOrders.Contains(sortOrder.ToLower()))
                    return StatusCode(400, new { message = AuditLogMessages.InvalidSortOrder });

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
                    return StatusCode(404, new { message = AuditLogMessages.NoLogs });

                return Ok(logs);
            }
            catch (UnauthorizedAccessException)
            {
                // Internal handling for 401
                return StatusCode(401, new { message = "Unauthorized access" });
            }
            catch (Exception ex)
            {
                // Internal handling for 500
                return StatusCode(500, new { message = AuditLogMessages.Error, detail = ex.Message });
            }
        }
    }
}
