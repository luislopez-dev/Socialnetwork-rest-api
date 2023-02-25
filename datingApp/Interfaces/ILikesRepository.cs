using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Helpers;

namespace datingApp.Interfaces
{
    public interface ILikesRepository
    {
        public Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        public Task<AppUser> GetUserWithLikes(int userId);
        public Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
        
    }
}