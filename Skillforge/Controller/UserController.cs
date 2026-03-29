using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Service;
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
    }
}
