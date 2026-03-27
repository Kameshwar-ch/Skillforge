using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Service;
using Skillforge.Utility;
[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
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