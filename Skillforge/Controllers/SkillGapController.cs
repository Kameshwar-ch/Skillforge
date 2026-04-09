using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace Skillforge.Controller
{
    [Authorize]
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
        public async Task<IActionResult> GetSkillGaps([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _skillGapService.GetFilteredGapsAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching skill gaps.", details = ex.Message });
            }
        }
    }
}