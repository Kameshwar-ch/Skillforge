using System;
using Skillforge.Dto;
namespace Skillforge.Service;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync();
}
