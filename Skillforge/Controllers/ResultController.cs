using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Skillforge.Domain;
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
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }

    

        [HttpGet("{assessmentId}")]
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
        public async Task<IActionResult> GetResultsByAssessment(int assessmentId)
        {
            try
            {
                var results = await _resultService.GetResultsByAssessmentAsync(assessmentId);

            if (results == null || results.Count == 0)
                return NotFound(new { message = "No results found for this assessment" });

            return Ok(results);
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPut("{assessmentId}/{employeeId}")]
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
        public async Task<IActionResult> UpdateResult(int assessmentId, int employeeId, [FromBody] UpdateResultDto dto)
        {
            try
            {
                var claim = User.FindFirst("id");
                if (claim == null)
                return Unauthorized("User ID not found");

                int reviewerId = int.Parse(claim.Value);

                await _resultService.UpdateResultAsync(assessmentId, employeeId, dto, reviewerId);

                return Ok(new { message = "Result updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{assessmentId}/{employeeId}")]
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
        public async Task<IActionResult> DeleteResult(int assessmentId, int employeeId)
        {
            try
            {
                var claim = User.FindFirst("id");
                if (claim == null)
                    return Unauthorized("User ID not found");

                int reviewerId = int.Parse(claim.Value);

                await _resultService.DeleteResultAsync(assessmentId, employeeId, reviewerId);

                return Ok(new
                {
                    message = "Result deleted successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Something went wrong" });
            }
        }
    }
}
