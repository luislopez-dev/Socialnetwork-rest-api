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
        private readonly IUnitOfWork _uofWork;
        public LikesController(IUnitOfWork uofWork)
        {
            _uofWork = uofWork;
        }
        
        [HttpPost("username")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _uofWork.UserRepository.GetUserByUserNameAsync(username);
            var sourceUser = await _uofWork.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
            {
                return NotFound();
            }
            if (sourceUser.UserName == username)
            {
                return BadRequest("You can not like yourself");
            }
            var userLike = await _uofWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            userLike = new UserLike {
                SourseUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            sourceUser.LikedByUsers.Add(userLike);
            if (await _uofWork.Complete())
            {
                return Ok();
            }
            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _uofWork.LikesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }
    }
}