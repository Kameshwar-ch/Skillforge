using System;
using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using SkillForgeLibrary.Models;

namespace Skillforge.Repository;

public class UserRepository : IUserRepository
{
    SkillForgeDB _context = new SkillForgeDB();
    public async Task<List<User>> GetAllUsersAsync()
    {
        List<User> users = await _context.Users.ToListAsync();
        if(users.Count ==0)
        {
            throw new Exception(Utility.ErrorMessages.UsersNotFound);
        }
        return users;
    }
}
