using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Service;

namespace Skillforge.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class CompetencyController : ControllerBase
	{
		private readonly ICompetencyService _competencyService;

		public CompetencyController(ICompetencyService competencyService)
		{
			_competencyService = competencyService;
		}

		[HttpGet("matrix")]
		[Authorize(Roles = "Manager,HR,Admin")]
		public async Task<IActionResult> GetCompetencyMatrix([FromQuery] CompetencyMatrixSearchDto searchDto)
		{
			var result = await _competencyService.GetCompetencyMatrixAsync(searchDto);
			return Ok(result);
		}
	}
}
