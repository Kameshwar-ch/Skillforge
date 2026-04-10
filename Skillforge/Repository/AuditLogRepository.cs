using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
using Skillforge.Dto;

namespace Skillforge.Repository
{
    /// <summary>
    /// EF Core implementation of IAuditLogRepository.
    /// Applies filters and sorting using enums.
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly SkillForgeDB _context;

        public AuditLogRepository(SkillForgeDB context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsFilteredAsync(AuditLogFilterRequestDto request)
        {
            IQueryable<AuditLog> query = _context.AuditLogs.AsQueryable();

            // Apply filters
            if (request.AuditID.HasValue)
                query = query.Where(x => x.AuditID == request.AuditID.Value);

            if (request.UserID.HasValue)
                query = query.Where(x => x.UserID == request.UserID.Value);

            if (!string.IsNullOrWhiteSpace(request.Resource))
                query = query.Where(x => x.Resource == request.Resource);

            if (!string.IsNullOrWhiteSpace(request.Action))
                query = query.Where(x => x.Action == request.Action);

            if (request.Timestamp.HasValue)
                query = query.Where(x => x.Timestamp == request.Timestamp.Value);
            // Apply sorting based on enums
           if (request.SortOrder == SortOrder.asc)
            {
                query = query
                    .OrderBy(p => p.AuditID)
                    .ThenBy(p => p.UserID)
                    .ThenBy(p => p.Timestamp);
            }
            else
            {
                query = query
                    .OrderByDescending(p => p.AuditID)
                    .ThenByDescending(p => p.UserID)
                    .ThenByDescending(p => p.Timestamp);
            }
            return await query.ToListAsync();
        }
    }
}
