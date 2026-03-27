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
        IUserService userService;
        public UserController(IUserService service)
        {
            userService = service;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {  
                List<UserResponseDto> users = await userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
