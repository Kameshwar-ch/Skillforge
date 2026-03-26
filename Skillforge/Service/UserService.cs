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

    public async Task<bool> UpdateUser(int userId, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user == null)
            return false;

        user.Name = request.Name;
        user.Role = request.Role;
        user.Phone = request.Phone;
        user.Status = request.Status;

        _userRepository.UpdateUser(user);

        return true;
    }
}