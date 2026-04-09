using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
using Skillforge.Utility;

namespace Skillforge.Repository
{
    /// <summary>
    /// Repository implementation for accessing immutable AuditLog records.
    /// Provides paginated and sortable queries.
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly SkillForgeDB _context;

        public AuditLogRepository(SkillForgeDB context)
        {
            _context = context;
        }

        public async Task<List<AuditLog>> GetAuditLogsPagedAsync(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = _context.AuditLogs.AsQueryable();

            // Default sort by Timestamp if no valid sortBy provided
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "Timestamp" : sortBy;

            query = sortOrder.ToLower() == "asc"
                ? query.OrderBy(e => EF.Property<object>(e, sortBy))
                : query.OrderByDescending(e => EF.Property<object>(e, sortBy));

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<AuditLog?> GetAuditLogByIdAsync(int auditId)
        {
            return await _context.AuditLogs.FirstOrDefaultAsync(a => a.AuditID == auditId);
        }
    }
}
