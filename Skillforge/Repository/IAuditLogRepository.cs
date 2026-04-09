using Skillforge.Domain;

namespace Skillforge.Repository
{
    public interface IAuditLogRepository
    {
        /// <summary>
        /// Retrieves audit logs with filters, pagination, and sorting.
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortBy">Sort field (AuditID, UserID, Resource, Action, Timestamp)</param>
        /// <param name="sortOrder">Sort order (asc/desc)</param>
        /// <param name="auditId">Filter by AuditID</param>
        /// <param name="userId">Filter by UserID</param>
        /// <param name="resource">Filter by Resource</param>
        /// <param name="action">Filter by Action</param>
        /// <param name="timestamp">Filter by Timestamp</param>
        /// <returns>List of audit logs</returns>
        Task<List<AuditLog>> GetAuditLogsFilteredAsync(
            int page, int pageSize, string sortBy, string sortOrder,
            int? auditId, int? userId, string? resource, string? action, DateTime? timestamp);
    }
}
