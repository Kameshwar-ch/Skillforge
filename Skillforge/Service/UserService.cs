using Skillforge.Domain;
using Skillforge.Repository;
using Skillforge.Service;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    /// <summary>
    /// Updates an existing user's information based on the provided userId.
    /// Retrieves the user from the repository, applies the allowed updates,
    /// and persists the changes. Returns false if the user does not exist.
    /// </summary>
    /// <param name="request">
    /// DTO containing the user details that are allowed to be updated, including the Id.
    /// </param>
    /// <returns>
    /// True if the user was successfully updated; false if the user was not found.
    /// </returns>


    public async Task<bool> UpdateUser(int id, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user == null)
            return false;

        user.Name = request.Name;
        user.Role = request.Role;
        user.Phone = request.Phone;
        user.Status = request.Status;

        return await _userRepository.UpdateUser(user);
        
    }
}