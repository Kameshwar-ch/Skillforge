using Skillforge.Domain;

namespace Skillforge.Repository;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email); 
    // Nullable Because The Email entered might not exists in the database

    // This will update the password in database
    Task<bool> UpdatePasswordAsync(string email, string hashedPassword);
}

