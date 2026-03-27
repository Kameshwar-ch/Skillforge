using System;
using Skillforge.Domain;

namespace Skillforge.Service;

public interface IUserService
{
    Task <User ?> Authenticate(string email, string password);
}
