namespace Skillforge.Dto;

// This DTO is used to is Just to get the Email from Database
public class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}

// This DTO is used to take the response from user and update the database
