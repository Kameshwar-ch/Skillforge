using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service
{
	public class CompetenyService : ICompetencyService
	{
		private readonly ICompetencyRepository _repository;
		private readonly IAuditService _auditService;
		public CompetenyService(ICompetencyRepository repository, IAuditService auditService)
		{
			_repository = repository;
			_auditService = auditService;
		}
		public async Task<List<CompetencyMatrixDto>> GetCompetencyMatrixAsync(CompetencyLevel? filterLevel)
		{
			try
			{
				var employees = await _repository.GetEmployeesWithSkillsAsync();
				return employees.Select(x => new CompetencyMatrixDto
				{
					EmployeeId = x.UserID,
					EmployeeName = x.Name,
					Skills = x.SkillGaps
					.Where(sg => filterLevel == null || sg.Competency.Level == filterLevel)
					.Select(sg => new EmployeeSkillDto
					{
						SkillName = sg.Competency.Name,
						Level = sg.Competency.Level.ToString()
					}).ToList()
				})
				.Where(dto => dto.Skills.Any() || filterLevel == null)
				.ToList();
			}
			catch (Exception ex)
			{
				await _auditService.LogAsync(null, "READ_FAILURE", $"Competency/Matrix Error: {ex.Message}");

				throw;
			}
		}

	}
}
