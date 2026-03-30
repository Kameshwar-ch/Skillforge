using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Service;
using Skillforge.Domain;
using Skillforge.Service;
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

		[HttpPost("Register")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UserRegister(UserRequestDto userRequestDto)
		{
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

    }
}
