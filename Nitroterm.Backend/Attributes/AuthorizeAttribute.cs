using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;

namespace Nitroterm.Backend.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        User? user = (User?) context.HttpContext.Items["User"];
        if (user != null) return;

        context.Result = new JsonResult(
            new ErrorResultDto("unauthorized", "You need to be logged in to access this resource"))
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }
}