using System;
using Microsoft.EntityFrameworkCore;
using Skillforge.Domain;
using SkillForgeLibrary.Models;

namespace Skillforge.Service;

/// <summary>
/// A concrete implementation of the IUserService that utilizes Entity Framework Core 
/// to handle user identification and cryptographic credential verification.
/// </summary>

public class EFUserRepository : IUserRepository
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

    private readonly SkillForgeDB _context;

    public EFUserRepository(SkillForgeDB context)
    {
        _context = context;
    }

    public async Task<User?> Authenticate(string email, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            bool isValidUser = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
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
