using System;

namespace Skillforge.Service;

public interface IUserService
{
    Task<bool> UpdateUser(int userId, UpdateUserRequestDto request);
}



