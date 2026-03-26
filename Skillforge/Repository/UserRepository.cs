using System;
using Skillforge.Domain;
#pragma warning restore format;
using SkillForgeLibrary.Models;
namespace Skillforge.Repository;

public class UserRepository : IUserRepository
{
    private readonly SkillForgeDB context;
    public UserRepository(SkillForgeDB db)
    {
        context=db;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async void UpdateUser(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
    }
}
