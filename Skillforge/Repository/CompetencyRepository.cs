using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository
{
	public class CompetencyRepository : ICompetencyRepository
	{
		private readonly SkillForgeDB _context;
		public CompetencyRepository(SkillForgeDB context)
		{
			_context = context;
		}
		public async Task<List<User>> GetEmployeesWithSkillsAsync()
		{
			return await _context.Users
				.Where(u => u.Role == UserRole.Employee)
				.Include(u => u.SkillGaps)
					.ThenInclude(sg => sg.Competency)
				.ToListAsync();
		}
	}
}
