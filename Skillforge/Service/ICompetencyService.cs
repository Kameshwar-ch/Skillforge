using Skillforge.Domain;
using Skillforge.Dto;

namespace Skillforge.Service
{
	public interface ICompetencyService
	{
		Task<List<CompetencyMatrixDto>> GetCompetencyMatrixAsync(CompetencyLevel? filterLevel);

	}
}
