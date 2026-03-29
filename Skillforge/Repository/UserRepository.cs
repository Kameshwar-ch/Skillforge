using System;
using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using SkillForgeLibrary.Models;
namespace Skillforge.Repository;

public class UserRepository : IUserRepository
{
    private readonly SkillForgeDB context;
    public UserRepository(SkillForgeDB _context)
    {
        context = _context;
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
