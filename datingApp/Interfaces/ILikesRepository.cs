using datingApp.DTOs;
using datingApp.Entities;

namespace datingApp.Interfaces
{
    public interface ILikesRepository
    {
        public Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        public Task<AppUser> GetUserWithLikes(int userId);
        public Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
        
    }
}