using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Dto;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/v1/users")]
public class UsersController : ControllerBase
{
    [HttpGet("/api/v1/user/{id}")]
    public object Get(int id)
    {
        using NitrotermDbContext db = new();

        Database.Models.User? dbUser = db.Users
            .Include(user => user.Product)
            .FirstOrDefault(user => user.Id == id);
        
        if (dbUser == null) return NotFound();

        return new UserDto(dbUser.Id, dbUser.Username,
            new ProductDto
            {
                Id = dbUser.Product.Id, Color = dbUser.Product.Color, Slug = dbUser.Product.Slug,
                Title = dbUser.Product.Title
            });
    }
}