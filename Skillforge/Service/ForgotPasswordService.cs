using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service;

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly IUserRepository UserRepository;

    public ForgotPasswordService(IUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    // Check if the email exists in the database
    public async Task<ApiResponseDto> VerifyEmailAsync(ForgotPasswordRequestDto dto)
    {

        if (string.IsNullOrWhiteSpace(dto.Email))
            return ApiResponseDto.FailResponse("Email is required.");

        // Check 
        var user = await UserRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            return ApiResponseDto.FailResponse("No account found with this email.");

        return ApiResponseDto.SuccessResponse("Email verified. You can now reset your password.");
    }

    //  Update the password
    public async Task<ApiResponseDto> ResetPasswordAsync(ResetPasswordDto dto)
    {
        // Checking Weather 
        if (string.IsNullOrWhiteSpace(dto.Email))
            return ApiResponseDto.FailResponse("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
            return ApiResponseDto.FailResponse("New password is required.");

        if (dto.NewPassword.Length < 6)
            return ApiResponseDto.FailResponse("Password must be at least 6 characters.");

        if (dto.NewPassword != dto.ConfirmPassword)
            return ApiResponseDto.FailResponse("Passwords do not match.");

        // Here once more we will check whether the Email entered for reset exists or not 
        var user = await UserRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            return ApiResponseDto.FailResponse("No account found with this email.");

        // The New password Entered Will be hashed here
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        var updated = await UserRepository.UpdatePasswordAsync(dto.Email, hashedPassword);

        return updated
            ? ApiResponseDto.SuccessResponse("Password updated successfully.")
            : ApiResponseDto.FailResponse("Something went wrong. Please try again.");
    }
}
