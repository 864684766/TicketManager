using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicketManager.Util;

namespace TicketManagerServer.Attributes
{
    public class AutoAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var _redisdb = new RedisHelper(configuration!);
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var redisToken = await _redisdb.GetAsync("IdentityKey");
            if (string.IsNullOrWhiteSpace(redisToken) || string.IsNullOrWhiteSpace(token))
            {
                context.Result = new UnauthorizedResult(); // 返回 401
            }
            else if (token != redisToken) 
            {
                context.Result = new UnauthorizedResult(); // 返回 401
            }
        }
    }
}
