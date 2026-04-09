using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repository;

        public AuditLogService(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<AuditLogResponseDto>> GetAuditLogsFilteredAsync(AuditLogFilterRequestDto request)
        {
            // Call repository with filters
            var logs = await _repository.GetAuditLogsFilteredAsync(
                request.Page, request.PageSize, request.SortBy, request.SortOrder,
                request.AuditID, request.UserID, request.Resource, request.Action, request.Timestamp);

            // If no records found, throw error
            if (!logs.Any())
                throw new Exception("No audit logs found for the given filter conditions.");

            // Map domain to response DTO
            var items = logs.Select(log => new AuditLogResponseDto
            {
                AuditID = log.AuditID,
                UserID = log.UserID,
                Action = log.Action,
                Resource = log.Resource,
                Timestamp = log.Timestamp
            });

            // Return paginated result
            return new PagedResult<AuditLogResponseDto>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(logs.Count / (double)request.PageSize),
                Items = items
            };
        }
    }
}
