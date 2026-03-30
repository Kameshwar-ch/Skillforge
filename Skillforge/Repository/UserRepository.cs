using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;
// using SkillForgeLibrary.Models;
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


	public async Task UserRegisterAsync(User user)
	{
		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();
	}


        public async Task<List<User>> GetAllUsersAsync()
    {
        List<User> users = await context.Users.ToListAsync();
        if(users.Count ==0)
        {
            throw new Exception(Utility.ErrorMessages.UsersNotFound);
        }
        return users;
    }
}

