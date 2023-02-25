using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Interfaces;

namespace datingApp.Data
{
    public class LikesRepository : ILikesRepository
    {
        Task<UserLike> ILikesRepository.GetUserLike(int sourceUserId, int targetUserId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<LikeDto>> ILikesRepository.GetUserLikes(string predicate, int userId)
        {
            throw new NotImplementedException();
        }

        Task<AppUser> ILikesRepository.GetUserWithLikes(int userId)
        {
            throw new NotImplementedException();
        }
    }
}