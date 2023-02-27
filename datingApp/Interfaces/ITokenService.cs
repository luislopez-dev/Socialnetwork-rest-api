using datingApp.Entities;

namespace datingApp.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}