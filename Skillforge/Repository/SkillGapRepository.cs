using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
using System.Linq;

namespace Skillforge.Repository
{
    public class SkillGapRepository : ISkillGapRepository
    {
        private readonly SkillForgeDB _context;

        public SkillGapRepository(SkillForgeDB context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SkillGap>> GetAllGapsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.SkillGaps
                .Include(g => g.Employee)
                .Include(g => g.Competency)
                .AsQueryable();

            // Support for Date Filters as per Jira Requirement
            if (startDate.HasValue)
                query = query.Where(g => g.DateIdentified >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(g => g.DateIdentified <= endDate.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<SkillGap>> GetGapsByEmployeeAsync(int employeeId)
        {
            return await _context.SkillGaps
                .Include(g => g.Competency)
                .Where(g => g.EmployeeID == employeeId)
                .ToListAsync();
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}