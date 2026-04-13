using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Service;
using Skillforge.Utility;
using Skillforge.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace Skillforge.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet("GetAll")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                List<UserResponseDto> users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
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
        [Authorize(Roles = nameof(UserRole.Admin))]
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

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserRegister(UserRequestDto userRequestDto)
        {
            Console.WriteLine("HELLO IN USER REGISTER CONTROLLER");
            try
            {
                var (success, errorMessage) = await _userService.UserRegisterAsync(userRequestDto);

                // Return 400 if email already exists or any validation fails
                if (!success)
                    return BadRequest(new { message = errorMessage });

                return StatusCode(201, new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpDelete("{userId}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid User ID");
                }

                var deleted = await _userService.DeleteUser(userId);

                if (!deleted)
                {
                    return NotFound(DeleteUserMessages.Delete.NotFound);
                }

                return Ok(DeleteUserMessages.Delete.Success);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
