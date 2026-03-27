using Skillforge.Domain;
using Skillforge.Repository;
using Skillforge.Service;
//using System.Threading.Tasks;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
   public async Task<bool> DeleteUser(int userId)
{
    return await _userRepository.DeleteUser(userId);
}
}