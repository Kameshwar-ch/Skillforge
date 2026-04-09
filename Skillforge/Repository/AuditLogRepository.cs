using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly SkillForgeDB _context;

        public AuditLogRepository(SkillForgeDB context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAuditLogsFilteredAsync(
            int page, int pageSize, string sortBy, string sortOrder,
            int? auditId, int? userId, string? resource, string? action, DateTime? timestamp)
        {
            var query = _context.AuditLogs.AsQueryable();

            // Apply filters
            if (auditId.HasValue)
                query = query.Where(a => a.AuditID == auditId.Value);

            if (userId.HasValue)
                query = query.Where(a => a.UserID == userId.Value);

            if (!string.IsNullOrWhiteSpace(resource))
                query = query.Where(a => a.Resource.Contains(resource));

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action.Contains(action));

            if (timestamp.HasValue)
                query = query.Where(a => a.Timestamp.Date == timestamp.Value.Date);

            // Sorting
            switch (sortBy)
            {
                case "AuditID":
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.AuditID) : query.OrderByDescending(a => a.AuditID);
                    break;
                case "UserID":
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.UserID) : query.OrderByDescending(a => a.UserID);
                    break;
                case "Resource":
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.Resource) : query.OrderByDescending(a => a.Resource);
                    break;
                case "Action":
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.Action) : query.OrderByDescending(a => a.Action);
                    break;
                case "Timestamp":
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.Timestamp) : query.OrderByDescending(a => a.Timestamp);
                    break;
                default:
                    throw new ArgumentException("Invalid sort field. Allowed: AuditID, UserID, Resource, Action, Timestamp.");
            }

            // Pagination
            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
