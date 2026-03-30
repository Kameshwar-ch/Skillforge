using Skillforge.Domain;

namespace Skillforge.Repository;

public interface IUserRepository
{
	public Task UserRegisterAsync(User user);

	// Nullable Because The Email entered might not exists in the database
	Task<User?> GetByEmailAsync(string email); 
}

