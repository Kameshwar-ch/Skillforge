using System;
using Skillforge.Domain;

namespace Skillforge.Service;

public interface IUserRepository
{
    Task <User ?> Authenticate(string email, string password);
}
