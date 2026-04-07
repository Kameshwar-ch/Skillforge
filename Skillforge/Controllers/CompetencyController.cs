using Microsoft.AspNetCore.Mvc;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Service;
using Microsoft.AspNetCore.Authorization;

namespace Skillforge.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CompetencyController : ControllerBase
	{
		private readonly ICompetencyService _service;
		public CompetencyController(ICompetencyService service)
		{
			_service = service;
		}

		[HttpGet("matrix")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[Authorize(Roles = "Manager,HR")]
		public async Task<ActionResult<List<CompetencyMatrixDto>>> GetMatrix([FromQuery] CompetencyLevel? level = null)
		{
			try
			{
				var result = await _service.GetCompetencyMatrixAsync(level);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message });
			}
		}
	}

}
