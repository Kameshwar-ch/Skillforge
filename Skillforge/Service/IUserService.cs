using System;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
public interface IUserService
{
	public Task<(bool Success, string ErrorMessage)> UserRegisterAsync(UserRequestDto userRequestDto);

}
