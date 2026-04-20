using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Service;
using Skillforge.Utility;

namespace Skillforge.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly ICourseService _courseService;
		public CourseController(ICourseService courseService)
		{
			_courseService = courseService;	
		}
		[HttpPost("{cid}/modules")]
		[Authorize(Roles = "Trainer")]
		public async Task<IActionResult> AddModule(int cid, [FromBody] CreateModuleDto dto)
		{
			var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			int? trainerId = null;
			if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
			{
				trainerId = id;
			}
			try
			{
				var moduleId = await _courseService.CreateModuleAsync(cid, dto, trainerId);

				return Ok(new
				{
					Message = CourseMessages.ModuleCreated,
					ModuleId = moduleId
				});
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An internal server error occurred.");
				
			}
		}
	}
}
