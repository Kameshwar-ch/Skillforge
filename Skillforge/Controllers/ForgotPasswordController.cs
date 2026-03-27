using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skillforge.Dto;
using Skillforge.Service;

namespace Skillforge.Controllers;

[ApiController]
[Route("User/[controller]")]
public class ForgotPasswordController : ControllerBase
{
    private readonly IForgotPasswordService ForgetPasswordService;

    public ForgotPasswordController(IForgotPasswordService forgotPasswordService)
    {
        ForgetPasswordService = forgotPasswordService;
    }

    // POST User/forgotpassword/verify-email
    [HttpPost("verifyemail")]
    public async Task<IActionResult> VerifyEmail([FromBody] ForgotPasswordRequestDto dto)
    {
        var result = await ForgetPasswordService.VerifyEmailAsync(dto);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    // POST User/forgotpassword/reset-password
    [HttpPost("resetpassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await ForgetPasswordService.ResetPasswordAsync(dto);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}

