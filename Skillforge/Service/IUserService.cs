using System;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
namespace Skillforge.Service;

public interface IUserService
{
    Task<bool> UpdateUser(int id,UpdateUserRequestDto request);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    public Task<(bool Success, string ErrorMessage)> UserRegisterAsync(UserRequestDto userRequestDto);

}
