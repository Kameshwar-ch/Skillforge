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

        [HttpDelete("{userId}")]
//[Authorize(Roles = "Admin")]
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
        return StatusCode(500,DeleteUserMessages.Delete.Error);
    }
}

    }
}
