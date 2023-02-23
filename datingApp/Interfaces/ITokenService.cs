using datingApp.Entities;

namespace datingApp.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}