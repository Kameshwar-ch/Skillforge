using Skillforge.Domain;

namespace Skillforge.Repository
{
	public interface ICompetencyRepository
	{
		Task<List<User>> GetEmployeesWithSkillsAsync();
	}
}
