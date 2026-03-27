using System;
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

    /// <summary>
    /// Retrieves a user entity from the database using the specified userId.
    /// Returns null if the user does not exist.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the user to retrieve.
    /// </param>
    /// <returns>
    /// The User entity if found; otherwise null.
    /// </returns>

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    /// <summary>
    /// Updates the given user entity in the database and persists the changes.
    /// </summary>
    /// <param name="user">
    /// The user entity containing updated values.
    /// <returns>
    /// True once the update operation is completed successfully.
    /// </returns>

    public async Task<bool> UpdateUser(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return true;
    }
}
