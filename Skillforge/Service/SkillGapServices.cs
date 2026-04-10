using Skillforge.Dto;
using Skillforge.Repository;
using System.Linq;

namespace Skillforge.Service
{
    public class SkillGapService : ISkillGapService
    {
        private readonly ISkillGapRepository _repository;

        public SkillGapService(ISkillGapRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SkillGapResponseDto>> GetFilteredGapsAsync(DateTime? startDate, DateTime? endDate)
        {
            var gaps = await _repository.GetAllGapsAsync(startDate, endDate);
            
            return gaps.Select(g => new SkillGapResponseDto
            {
                SkillGapID = g.SkillGapID,
                EmployeeName = g.Employee.Name,
                CompetencyName = g.Competency.Name,
                GapLevel = g.GapLevel.ToString(),
                DateIdentified = g.DateIdentified
            }).ToList();
        }
    }
}