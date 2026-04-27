using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service
{
	public class CompetencyService : ICompetencyService
	{
		private readonly ICompetencyRepository _repository;

		public CompetencyService(ICompetencyRepository repository)
		{
			_repository = repository;
		}

		public async Task<List<CompetencyMatrixDto>> GetCompetencyMatrixAsync(CompetencyMatrixSearchDto searchDto)
		{
			var employees = await _repository.GetEmployeesWithSkillsAsync();

			return employees
				.Where(u => (searchDto.EmployeeId == null || u.UserID == searchDto.EmployeeId) && (string.IsNullOrWhiteSpace(searchDto.EmployeeName) ||
							u.Name.Contains(searchDto.EmployeeName, StringComparison.OrdinalIgnoreCase)))
				.Select(u => new CompetencyMatrixDto
				{
					EmployeeId = u.UserID,
					EmployeeName = u.Name,
					Skills = u.SkillGaps
						.Where(sg =>
							(searchDto.Level == null || sg.Competency.Level == searchDto.Level) &&
							(string.IsNullOrWhiteSpace(searchDto.SkillName) ||
							 sg.Competency.Name.Contains(searchDto.SkillName, StringComparison.OrdinalIgnoreCase))
						)
						.Select(sg => new EmployeeSkillDto
						{
							SkillName = sg.Competency.Name,
							Level = sg.Competency.Level.ToString()
						}).ToList()
				})
				.Where(dto => dto.Skills.Any() ||
							 (string.IsNullOrWhiteSpace(searchDto.EmployeeName) &&
							  string.IsNullOrWhiteSpace(searchDto.SkillName) &&
							  searchDto.Level == null))
				.ToList();
		}
	}
}
