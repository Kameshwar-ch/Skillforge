using Skillforge.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skillforge.Service
{
    public interface ISkillGapService
    {
        Task<List<SkillGapResponseDto>> GetFilteredGapsAsync(DateTime? startDate, DateTime? endDate);
    }
}