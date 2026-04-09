using Skillforge.Dto;

namespace Skillforge.Service
{
    /// <summary>
    /// Contract for immutable AuditLog service operations.
    /// Provides DTO mapping and business logic.
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Retrieves paginated and sortable audit logs mapped to DTOs.
        /// </summary>
        Task<PagedResult<AuditLogDto>> GetAuditLogsPagedAsync(int page, int pageSize, string sortBy, string sortOrder);

        /// <summary>
        /// Retrieves a single audit log by ID mapped to DTO.
        /// </summary>
        Task<AuditLogDto?> GetAuditLogByIdAsync(int auditId);
    }
}
