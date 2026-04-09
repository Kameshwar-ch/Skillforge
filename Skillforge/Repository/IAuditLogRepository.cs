using Skillforge.Domain;

namespace Skillforge.Repository
{
    /// <summary>
    /// Contract for accessing immutable AuditLog records.
    /// Provides read-only operations.
    /// </summary>
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Retrieves paginated and sortable audit logs.
        /// </summary>
        Task<List<AuditLog>> GetAuditLogsPagedAsync(int page, int pageSize, string sortBy, string sortOrder);

        /// <summary>
        /// Retrieves a single audit log by its unique ID.
        /// </summary>
        Task<AuditLog?> GetAuditLogByIdAsync(int auditId);
    }
}
