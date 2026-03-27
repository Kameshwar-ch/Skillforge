using System;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;

namespace Skillforge.Service;

public class UserService : IUserService
{
    IUserRepository userRepository;
    public UserService(IUserRepository rep)
    {
        userRepository = rep;
    }
    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
       List<User> users;
        try
        {
        users = await userRepository.GetAllUsersAsync();
            
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
