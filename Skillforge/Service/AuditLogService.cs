using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service
{
    /// <summary>
    /// Service layer implementation for immutable AuditLog records.
    /// Provides business logic and DTO mapping for paginated queries.
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogService(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<AuditLogDto>> GetAuditLogsPagedAsync(int page, int pageSize, string sortBy, string sortOrder)
        {
            var logs = await _repository.GetAuditLogsPagedAsync(page, pageSize, sortBy, sortOrder);

            return new PagedResult<AuditLogDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(logs.Count / (double)pageSize),
                Items = logs.Select(log => new AuditLogDto
                {
                    AuditID = log.AuditID,
                    UserID = log.UserID,
                    Action = log.Action,
                    Resource = log.Resource,
                    Timestamp = log.Timestamp
                })
            };
        }

        public async Task<AuditLogDto?> GetAuditLogByIdAsync(int auditId)
        {
            var log = await _repository.GetAuditLogByIdAsync(auditId);
            if (log == null) return null;

            return new AuditLogDto
            {
                AuditID = log.AuditID,
                UserID = log.UserID,
                Action = log.Action,
                Resource = log.Resource,
                Timestamp = log.Timestamp
            };
        }
    }
}
