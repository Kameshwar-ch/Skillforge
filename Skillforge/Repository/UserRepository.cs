using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using Skillforge.Domain;

namespace Skillforge.Repository;

public class UserRepository : IUserRepository
{
    private readonly SkillForgeDB _context;

    public UserRepository(SkillForgeDB context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    // Updates the database with 
    public async Task<bool> UpdatePasswordAsync(string email, string hashedPassword)
    {
        var user = await GetByEmailAsync(email);
        if (user == null) return false; // if user with email does not exists 

        user.Password = hashedPassword;
        return await _context.SaveChangesAsync() > 0;
    }
}

