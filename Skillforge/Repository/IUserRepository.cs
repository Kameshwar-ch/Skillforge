using System;
using Skillforge.Domain;

namespace Skillforge.Repository;

public interface IUserRepository
{
    Task <User ?> Authenticate(string email, string password);
    Task<List<User>> GetAllUsersAsync();
}

