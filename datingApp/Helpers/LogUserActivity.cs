using datingApp.Extensions;
using datingApp.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace datingApp.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async  Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
        {
            return;
        }
        var userId = resultContext.HttpContext.User.GetUserId();
        var uoWork = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await uoWork.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.Now;
        await uoWork.Complete();
    }
}