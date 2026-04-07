using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository
{
	public class CompetencyRepository : ICompetencyRepository
	{
		private readonly SkillForgeDB context;
		public CompetencyRepository(SkillForgeDB context)
		{
			this.context = context;
		}
		public async Task<List<User>> GetEmployeesWithSkillsAsync()
		{
			return await context.Users
					.Where(u => u.Role == UserRole.Employee)
					.Include(sk => sk.SkillGaps)
					.ThenInclude(c => c.Competency)
					.ToListAsync();

		}

	}
}
