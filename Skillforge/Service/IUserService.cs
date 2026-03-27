using System;

namespace Skillforge.Service;

public interface IUserService
{
        Task<bool> DeleteUser(int userId);
}




