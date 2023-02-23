using datingApp.Data;
using datingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Controllers;

public class BuggyController : BaseApiController
{
    private readonly DataContext _dataContext;

    public BuggyController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [Authorize]
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var user = _dataContext.Users.Find(-1);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }
    
    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var user = _dataContext.Users.Find(-1);
        var userToReturn = user.ToString();
        return userToReturn;
    }
    
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }
}