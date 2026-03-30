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
   /// <summary>
    /// Updates an existing user's information based on the provided userId.
    /// Retrieves the user from the repository, applies the allowed updates,
    /// and persists the changes. Returns false if the user does not exist.
    /// </summary>
    /// <param name="request">
    /// DTO containing the user details that are allowed to be updated, including the Id.
    /// </param>
    /// <returns>
    /// True if the user was successfully updated; false if the user was not found.
    /// </returns>
    public async Task<bool> UpdateUser(int id, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
            return false;

        user.Name = request.Name;
        user.Role = request.Role;
        user.Phone = request.Phone;
        user.Status = request.Status;

        return await _userRepository.UpdateUser(user);
        
    }
    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
       List<User> users;
        try
        {
            users = await _userRepository.GetAllUsersAsync();   
        }
        catch(Exception)
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
