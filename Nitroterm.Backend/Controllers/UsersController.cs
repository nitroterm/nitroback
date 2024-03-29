using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

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
        
        if (dbUser == null) return NotFound(new ErrorResultDto("not_found", "user not found"));

        return new ResultDto<UserDto>(new UserDto(dbUser));
    }

    [HttpPut("/api/nitroterm/v1/user")]
    [Authorize]
    public object Put([FromBody] UserEditDto dto)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;

        if (!string.IsNullOrWhiteSpace(dto.Username))
        {
            if (db.Users.Any(u => u.Username == dto.Username))
                return new ErrorResultDto("already_taken", "username is already taken");
            
            user.Username = dto.Username;
        }
        if (!string.IsNullOrWhiteSpace(dto.Bio)) user.Bio = dto.Bio;
        if (dto.ProductId != null) user.Product = db.Products.Find(dto.ProductId!);

        db.Update(user);
        db.SaveChanges();

        return new ResultDto<UserDto>(new UserDto(user));
    }
}