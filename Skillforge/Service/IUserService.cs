using System;
namespace Skillforge.Service;

public interface IUserService
{
	public Task<(bool Success, string ErrorMessage)> UserRegisterAsync(UserRequestDto userRequestDto);

}
