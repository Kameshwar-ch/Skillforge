using System;
using Skillforge.Domain;
namespace Skillforge.Repository;
public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
}
