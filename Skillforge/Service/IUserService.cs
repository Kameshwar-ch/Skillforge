using System;

namespace Skillforge.Service;

public interface IUserService
{
    Task<bool> UpdateUser(int id,UpdateUserRequestDto request);
}



