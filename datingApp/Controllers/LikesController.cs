using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Extensions;
using datingApp.Helpers;
using datingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly ILikesRepository likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            this.userRepository = userRepository;
            this.likesRepository = likesRepository;
        }

        [HttpPost("username")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await userRepository.GetUserByUserNameAsync(username);
            var sourceUser = await likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
            {
                return NotFound();
            }
            if (sourceUser.UserName == username)
            {
                return BadRequest("You can not like yourself");
            }
            var userLike = await likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            userLike = new UserLike {
                SourseUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            sourceUser.LikedByUsers.Add(userLike);
            if (await userRepository.SaveAllAsync())
            {
                return Ok();
            }
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await likesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }
    }
}