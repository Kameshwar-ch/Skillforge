namespace  Skillforge.Repository;
using Skillforge.Domain;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(int userId);
    void UpdateUser(User user);
    // void Savechanges();
}
