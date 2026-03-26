using System;

namespace Skillforge.Service;

public interface IUserService
{
    bool UpdateUser(int userId, UpdateUserRequestDto request);
}



