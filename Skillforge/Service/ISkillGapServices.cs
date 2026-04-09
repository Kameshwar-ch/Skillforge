using Skillforge.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skillforge.Service
{
    public interface ISkillGapService
    {
        // Updated to include all 5 filter parameters
        Task<List<SkillGapResponseDto>> GetFilteredGapsAsync(
            DateTime? startDate, 
            DateTime? endDate, 
            int? employeeId, 
            int? competencyId, 
            int? gapLevel);
    }
}