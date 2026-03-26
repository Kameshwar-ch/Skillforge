namespace  Skillforge.Repository;
using Skillforge.Domain;

public interface IUserRepository
{
    User GetUserById(int userId);
    void UpdateUser(User user);
    void Savechanges();
}
