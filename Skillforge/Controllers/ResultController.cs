using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Skillforge.Controllers
{
    [Route("api/v1/Result")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;
        public ResultController(IResultService resultService)
        {
            _resultService = resultService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> SubmitAssessmentResult([FromBody] SubmitAssessmentResultDto dto)
        {
            try
            {
                
                var claim = User.FindFirst("id");
                if (claim == null)
                {
                    return Unauthorized("User ID not found in token");
                }

                int reviewerUserId = int.Parse(claim.Value);

                await _resultService.SubmitResultAsync(dto, reviewerUserId);

                return StatusCode(201,new
                {
                    message = "Assessment result submitted successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
