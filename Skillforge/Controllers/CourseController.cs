using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Dto;
using Skillforge.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skillforge.Utility;
using System;

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
                var result = await _courseService.CreateCourseAsync(request);
                return CreatedAtAction(nameof(CreateCourse), new { id = result.CourseID }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal server error occurred. Please try again.");
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
    }
}