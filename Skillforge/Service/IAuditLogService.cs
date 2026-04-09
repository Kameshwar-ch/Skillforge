using Skillforge.Dto;

namespace Skillforge.Service
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Retrieves audit logs with filters, pagination, and sorting.
        /// </summary>
        /// <param name="request">Filter, pagination, and sorting options</param>
        /// <returns>Paged result of audit logs</returns>
        Task<PagedResult<AuditLogResponseDto>> GetAuditLogsFilteredAsync(AuditLogFilterRequestDto request);
    }
}
