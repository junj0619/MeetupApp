using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MeetupApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        /*
          ActionExecutingContext: means excute some logic during context executing 
          ActionExecutionDelegate: means excute some logic after context executed  
          In this case, we want to log user activity after request being exceuted
        */
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            /* 
               1) Wait current context execution completed 
               2) Get userId from Claim
               3) Update user LastActive to Now
            */
            var resultContext = await next();

            var userId = int.Parse(resultContext.HttpContext
                                    .User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var repo = resultContext.HttpContext.RequestServices.GetService<IMeetupRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;

            await repo.SaveAll();
        }
    }
}