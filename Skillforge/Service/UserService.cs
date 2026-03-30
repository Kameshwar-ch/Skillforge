using System;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Identity;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
namespace Skillforge.Service;

public class UserService : IUserService
{
     private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
       List<User> users;
        try
        {
            users = await _userRepository.GetAllUsersAsync();   
        }
        catch(Exception ex)
        {
            throw new Exception(Utility.ErrorMessages.UsersNotFound);
        } 
       List<UserResponseDto> userResponseDtos = new List<UserResponseDto>();
       foreach(User user in users)
        {
            UserResponseDto responseDto = new UserResponseDto
            { 
                UserID = user.UserID,
                UserName = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                RoleName = user.Role,
                Status = user.Status
            };
            userResponseDtos.Add(responseDto);
        }
        return userResponseDtos;
    }

    	public async Task<(bool Success, string ErrorMessage)> UserRegisterAsync(UserRequestDto userRequestDto)
	{
		//block duplicate email registrations before doing any DB write

		var existingUser = await _userRepository.GetByEmailAsync(userRequestDto.Email!);

		if (existingUser != null && existingUser.Status)
			return (false, "Email is already registered.");

		if (existingUser != null && !existingUser.Status)
			return (false, "Your account is inactive. Please contact support.");

		// Map DTO → Domain model, assigning default role and hashing the password
		var userModel = new User
		{
			Name = userRequestDto.Name,
			Role = UserRole.Employee,
			Email = userRequestDto.Email,
			Phone = userRequestDto.Phone,
			Password = BCrypt.Net.BCrypt.HashPassword(userRequestDto.Password),
			Status = true
		};


		await _userRepository.UserRegisterAsync(userModel);
		return (true, null!);
	}

}
