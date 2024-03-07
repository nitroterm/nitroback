using Microsoft.AspNetCore.Mvc;
using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Utilities;

public static class Extensions
{
    public static bool IsAuthenticated(this ControllerBase controller)
        => controller.Request.HttpContext.Items["User"] != null;
    
    public static bool User(this ControllerBase controller, Func<User?, bool> predicate)
        => predicate(controller.GetUser());
    
    public static User? GetUser(this ControllerBase controller)
        => (User?)controller.Request.HttpContext.Items["User"];
}