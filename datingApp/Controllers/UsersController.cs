using AutoMapper;
using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Extensions;
using datingApp.Helpers;
using datingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUnitOfWork _uoWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IMapper mapper, IPhotoService photoService, IUnitOfWork uoWork)
    {
        _uoWork = uoWork;
        _mapper = mapper;
        _photoService = photoService;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = await _uoWork.UserRepository.GetUserGender(User.GetUsername());
        userParams.CurrentUsername = User.GetUsername();
    
        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }
        
        var users = await _uoWork.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(
            users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));
        return Ok(users);
    }

    [Authorize(Roles = "Member")]
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _uoWork.UserRepository.GetMemberAsync(username);
    }
    
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var user = await _uoWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
        if (user == null)
        {
            return NotFound();
        }
        _mapper.Map(memberUpdateDto, user);
        if (await _uoWork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _uoWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
        if (user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }
        
        user.Photos.Add(photo);
        if (await _uoWork.Complete())
        {
            return CreatedAtAction(nameof(GetUser), 
                new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
        }
        return BadRequest("Problem uploading the photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _uoWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
        
        if (user == null) return NotFound();
        
        var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
        
        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("This is already your main photo");

        var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);

        if (currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _uoWork.Complete()) return NoContent();

        return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _uoWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
        
        var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
        
        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You can not delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);
        if (await _uoWork.Complete()) return Ok();
        return BadRequest("Problem uploading your photo");
    }
    
    [HttpPost("test")]
    public async Task<AppUser> GetPhotos()
    {
        return await _uoWork.UserRepository.GetUserByUserNameAsync(User.GetUsername());
    }
}