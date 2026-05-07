using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skillforge.Dto;
using Skillforge.Repository;
using Skillforge.Service;

namespace Skillforge.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        IEnrollmentService enrollmentService;
        public EnrollmentController(IEnrollmentService _enrollmentService)
        {
            enrollmentService = _enrollmentService;
        }
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Enroll(EnrollmentDto dto)
        {
            try
            {
                
                var empIdClaim = User.FindFirst("id")?.Value;

                if (empIdClaim == null)
                return Unauthorized("Invalid token");

                int employeeId = int.Parse(empIdClaim);
                var id = await enrollmentService.EnrollAsync(dto.CourseId, employeeId);
                return Created("", new { enrollmentId = id });
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
