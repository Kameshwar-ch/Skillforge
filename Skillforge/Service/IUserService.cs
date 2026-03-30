using System;
using Skillforge.Dto;
namespace Skillforge.Service;
public interface IUserService
{
    Task<bool> DeleteUser(int userId);
    Task<List<UserResponseDto>> GetAllUsersAsync();
}
