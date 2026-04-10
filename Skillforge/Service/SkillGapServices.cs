using Skillforge.Dto;
using Skillforge.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skillforge.Service
{
    public class SkillGapService : ISkillGapService
    {
        private readonly ISkillGapRepository _repository;

        public SkillGapService(ISkillGapRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SkillGapResponseDto>> GetFilteredGapsAsync(
            DateTime? startDate, 
            DateTime? endDate, 
            int? employeeId, 
            int? competencyId, 
            int? gapLevel)
        {
            // 1. Pass all filter parameters to the Repository
            var gaps = await _repository.GetAllGapsAsync(startDate, endDate, employeeId, competencyId, gapLevel);
            
            // 2. Map the entities to DTOs
            return gaps.Select(g => new SkillGapResponseDto
            {
                SkillGapID = g.SkillGapID,
                // Using null-conditional operator (?) to prevent crashes if navigation properties are null
                EmployeeName = g.Employee?.Name ?? "N/A",
                CompetencyName = g.Competency?.Name ?? "N/A",
                GapLevel = g.GapLevel.ToString(),
                DateIdentified = g.DateIdentified
            }).ToList();
        }
    }
}