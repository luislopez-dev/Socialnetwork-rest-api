using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Helpers;

namespace datingApp.Interfaces;

public interface IUserRepository
{
    public void Update(AppUser user);
    public Task<bool> SaveAllAsync();
    public Task<IEnumerable<AppUser>> GetUsersAsync();
    public Task<AppUser> GetUserByIdAsync(int id);
    public Task<AppUser> GetUserByUserNameAsync(string username);
    public Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    public Task<MemberDto> GetMemberAsync(string username);

}