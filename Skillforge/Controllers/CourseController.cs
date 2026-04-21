using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // 1. Added this namespace

namespace Skillforge.Controller
{
    [Route("api/v1/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
         [Authorize(Roles = "Admin,Trainer")] 
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault();

                return BadRequest(errorMessage);
            }

            try
            {
                var result = await _courseService.CreateCourseAsync(request);
                return CreatedAtAction(nameof(CreateCourse), new { id = result.CourseID }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "An internal server error occurred. Please try again.");
            }
        }
    }
}