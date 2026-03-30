using System;
using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using Skillforge.Service;
using SkillForgeLibrary.Models;
namespace Skillforge.Repository;

/// <summary>
/// A concrete implementation of the IUserService that utilizes Entity Framework Core 
/// to handle user identification and cryptographic credential verification.
/// </summary>
public class UserRepository : IUserRepository
{
    /// <summary>
    /// Authenticates a user by performing a two-stage verification process:
    /// 1. Retrieves the user record from the database using the provided email.
    /// 2. Validates the plain-text password against the stored BCrypt hash.
    /// </summary>
    /// <param name="email">The unique email address of the user attempting to log in.</param>
    /// <param name="password">The plain-text password provided by the user.</param>
    /// <returns>
    /// Returns the <see cref="User"/> entity if authentication is successful; 
    /// otherwise, returns <c>null</c> if the user is not found or the password is incorrect.
    /// </returns>
    /// <exception cref="Exception">Thrown when a database error or unexpected system failure occurs during the process.</exception>
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

    public async Task<User?> Authenticate(string email, string password)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            bool isValidUser = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isValidUser)
            {
                return null;
            }

            return user;
        }
        catch (System.Exception ex)
        {
            throw new Exception("No such User Exists. " + ex);
        }
    }
}
