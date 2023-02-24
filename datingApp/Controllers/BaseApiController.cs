using datingApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[Route("api/[controller]")]
[ApiController]
public class BaseApiController : Controller
{
}