using Skillforge.Dto;

namespace Skillforge.Service
{
	public interface ICompetencyService
	{
		Task<List<CompetencyMatrixDto>> GetCompetencyMatrixAsync(CompetencyMatrixSearchDto searchDto);
	}
}
