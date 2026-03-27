namespace  Skillforge.Repository;
using Skillforge.Domain;

public interface IUserRepository
{
    Task<bool> DeleteUser(int userId);
}
