using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Run4Prize.Filters
{
    public class CustomAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        public string? Roles { get; set; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity!.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        action = "Error",
                        controller = "Home",
                        error = "Forbidden",
                    })
                );
            }
            var claims = user.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .ToList();
            if (!string.IsNullOrEmpty(Roles))
            {
                var roles = Roles.Split(",");
                var flag = false;
                for (var i = 0; i < roles.Length; i++)
                {
                    var findClaim = claims.Where(it => it.Value == roles[i]).FirstOrDefault();
                    if (findClaim != null)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        action = "Error",
                        controller = "Home",
                        error = "Forbidden",
                    }));
                }
            }
        }
    }
}
