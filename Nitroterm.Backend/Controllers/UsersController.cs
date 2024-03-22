using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Dto;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/users")]
public class UsersController : ControllerBase
{
    [HttpGet("/api/nitroterm/v1/user/{id:int}")]
    [Authorize]
    public object Get(int id)
    {
        using NitrotermDbContext db = new();

        Database.Models.User? dbUser = db.Users
            .Include(user => user.Product)
            .FirstOrDefault(user => user.Id == id);
        
        if (dbUser == null) return NotFound();

        return new UserDto(dbUser);
    }
}