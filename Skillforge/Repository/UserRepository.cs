using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository;

public class UserRepository : IUserRepository
{
    private readonly SkillForgeDB context;

    public UserRepository(SkillForgeDB context)
    {
        this.context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    // Updates the database with 
    public async Task<bool> UpdatePasswordAsync(string email, string hashedPassword)
    {
        var user = await GetByEmailAsync(email);
        if (user == null) return false; // if user with email does not exists 

        user.Password = hashedPassword;
        return await context.SaveChangesAsync() > 0;
    }

	public async Task UserRegisterAsync(User user)
	{
		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();
	}

	//Returns true if any user already has the given email (used for duplicate check)

	//public async Task<User?> GetByEmailAsync(string email)
	//{
	//	return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
	//}
}

