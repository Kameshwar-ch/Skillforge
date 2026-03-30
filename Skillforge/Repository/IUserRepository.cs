using System;
using Skillforge.Domain;
namespace Skillforge.Repository;
public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int userId);
    Task<bool> UpdateUser(User user);
    Task<List<User>> GetAllUsersAsync();
}
