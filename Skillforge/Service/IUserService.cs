<<<<<<< HEAD
﻿using System;
using Skillforge.Domain;
using Skillforge.Dto;
using Skillforge.Repository;
public interface IUserService
{
	public Task<(bool Success, string ErrorMessage)> UserRegisterAsync(UserRequestDto userRequestDto);

=======
using System;
using Skillforge.Dto;
namespace Skillforge.Service;

public interface IUserService
{
    Task<List<UserResponseDto>> GetAllUsersAsync();
>>>>>>> 55e6e8186bf93cb46c07bbabf4f5611d773cec65
}
