using System;
using Skillforge.Domain;
namespace Skillforge.Repository;
public interface IUserRepository
{
    Task<bool> DeleteUser(int userId);
    Task<List<User>> GetAllUsersAsync();
}
