using System;
using Skillforge.Dto;
namespace Skillforge.Service;

public interface IUserService
{
    Task<bool> UpdateUser(int id,UpdateUserRequestDto request);
    Task<List<UserResponseDto>> GetAllUsersAsync();
}
