using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Skillforge.Domain;
using Microsoft.AspNetCore.Authorization;
using System;
using Skillforge.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        // Requirement: Auth read from Enum (No hardcoding)
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
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
                await _courseService.CreateCourseAsync(request);
                
                // Requirement: Return only a success string
                return Ok("Course successfully added."); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
            }
        }
        [HttpPost("{cid}/modules")]
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer))]
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
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("{courseID}")]
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Trainer) + "," + nameof(UserRole.Employee)    )]
        public async Task<IActionResult> GetCourseByID(int courseID)
        {
            try
            {
                var claim = User.FindFirst("id");
                if (claim == null)
                    return Unauthorized(new { message = "UnAuthorize User." });

                int userID = int.Parse(claim.Value);

                var result = await _courseService.GetCourseByIDAsync(courseID, userID);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, inner = ex.InnerException?.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] CourseFilterRequestDto request)
        {
            var result = await _courseService.GetCoursesAsync(request);
            return Ok(result);
        }
    }
}