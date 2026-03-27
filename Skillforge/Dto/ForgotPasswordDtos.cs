namespace Skillforge.Dto;

// This DTO is used to is Just to get the Email from Database
public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}

// This DTO is used to take the response from user and update the database
public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ApiResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    // This will just return the object of DTO with required Result type
    public static ApiResponseDto SuccessResponse(string message)
    {
        return new ApiResponseDto { Success = true, Message = message};
    }

    public static ApiResponseDto FailResponse(string message)
    {
        return new ApiResponseDto {Success = false, Message = message};
    }
}

