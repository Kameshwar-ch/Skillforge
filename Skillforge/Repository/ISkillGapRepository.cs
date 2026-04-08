using Skillforge.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using Skillforge.Data;
namespace Skillforge.Repository
{
    public interface ISkillGapRepository
    {
        Task<IEnumerable<SkillGap>> GetAllGapsAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<SkillGap>> GetGapsByEmployeeAsync(int employeeId);
        Task SaveAsync();
    }
}