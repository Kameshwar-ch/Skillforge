using Skillforge.Domain;


namespace Skillforge.Repository;


public interface IUserRepository
{
    Task<User?> Authenticate(string email, string password);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllUsersAsync();
    Task<bool> UpdatePasswordAsync(string email, string hashedPassword);
}

