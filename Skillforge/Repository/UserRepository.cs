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
    public User GetUserById(int userId)
    {
        return context.Users.FirstOrDefault(u => u.UserID == userId);
    }

    public void UpdateUser(User user)
    {
        context.Users.Update(user);
    }

    public void Savechanges()
    {
        context.SaveChanges();
    }
}
