using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Slidable.Shows
{
    [PublicAPI]
    public class BypassAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BypassAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "mark@rendle.io"),
                new Claim(SlidableClaimTypes.Handle, "rendle")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            context.User = new ClaimsPrincipal(identity);

            return _next(context);
        }
    }
}