using System;
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
   public async Task<bool> DeleteUser(int userId)
{
    return await _userRepository.DeleteUser(userId);
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
}
