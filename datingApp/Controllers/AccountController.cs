using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using datingApp.Data;
using datingApp.DTOs;
using datingApp.Entities;
using datingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace datingApp.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext dataContext, ITokenService tokenService, IMapper mapper)
    {
        _dataContext = dataContext;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _dataContext.Users.SingleOrDefaultAsync(user => user.UserName == loginDto.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username");
        }
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid Password");
            }
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {    
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }
        using var hmac = new HMACSHA512();

        // Temporary workaround while app is updated to .net core v7
       //   registerDto.DateOfBirth = new DateOnly(1988,06,22);

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;
        
        _dataContext.Users.Add(user);
        await _dataContext.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain)?.Url,
            KnownAs = user.KnownAs
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _dataContext.Users.AnyAsync(user => user.UserName == username.ToLower());
    }
}