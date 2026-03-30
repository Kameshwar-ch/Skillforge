using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Service;
using Skillforge.Utility;
namespace Skillforge.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("GetAll")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {  
                List<UserResponseDto> users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Allows an Admin to update an existing user's information.
        /// Performs basic request validation and delegates business logic to the service layer.
        /// </summary>
        /// <param name="id">Receives the target userId from the route and update data from the request body.</param>
        /// <param name="request">update user request containing the feilds that allowed to be updated </param>
        /// <returns></returns>
        [HttpPut("update/{id}")]
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool updated = await _userService.UpdateUser(id, request);

                if (!updated)
                {
                    return NotFound(UpdateMessages.NotFound);
                }

                return Ok(UpdateMessages.success);
            }
            catch (Exception)
            {
                return StatusCode(500, UpdateMessages.Error);
            }
        }
    }
}