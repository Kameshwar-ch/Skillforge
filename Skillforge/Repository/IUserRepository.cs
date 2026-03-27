namespace  Skillforge.Repository;
using Skillforge.Domain;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int userId);
    Task<bool> UpdateUser(User user);
}
