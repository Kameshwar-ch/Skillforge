using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Skillforge.Dto;
using Skillforge.Repository;
using Skillforge.Service;

namespace Skillforge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        IEnrollmentService enrollment;
        public EnrollmentController(IEnrollmentService _enrollment)
        {
            enrollment = _enrollment;
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> Enroll(EnrollmentDto dto)
        {
            try
            {
                var id = await enrollment.EnrollAsync(dto.CourseId, dto.EmployeeId);
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
