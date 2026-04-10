using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Skillforge.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SkillGapController : ControllerBase
    {
        private readonly ISkillGapService _skillGapService;

        public SkillGapController(ISkillGapService skillGapService)
        {
            _skillGapService = skillGapService;
        }

        [HttpGet]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> GetSkillGaps(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate,
            [FromQuery] int? employeeId,
            [FromQuery] int? competencyId,
            [FromQuery] int? gapLevel)
        {
            // Validation: One-line error string for the tester
            if (startDate.HasValue && endDate.HasValue && endDate < startDate)
            {
                return BadRequest("End date cannot be earlier than the start date.");
            }

            try
            {
                var result = await _skillGapService.GetFilteredGapsAsync(
                    startDate, 
                    endDate, 
                    employeeId, 
                    competencyId, 
                    gapLevel);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }
}