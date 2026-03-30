using Skillforge.Domain;

namespace Skillforge.Repository;

public interface IUserRepository
{
	public Task UserRegisterAsync(User user);
	// Nullable Because The Email entered might not exists in the database
	Task<User?> GetByEmailAsync(string email); 
    // This will update the password in database
    Task<List<User>> GetAllUsersAsync();

    Task<bool> UpdatePasswordAsync(string email, string hashedPassword);
    Task<User> GetUserByIdAsync(int id);
    Task<bool> UpdateUser(User user);
}
