using Skillforge.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skillforge.Repository
{
    public interface ISkillGapRepository
    {
        // Updated to include all filter parameters: dates, employee, competency, and level
        Task<IEnumerable<SkillGap>> GetAllGapsAsync(
            DateTime? startDate, 
            DateTime? endDate, 
            int? employeeId = null, 
            int? competencyId = null, 
            int? gapLevel = null);

        // Keep this for specific employee lookups
        Task<IEnumerable<SkillGap>> GetGapsByEmployeeAsync(int employeeId);

        // Standard save method
        Task SaveAsync();
    }
}