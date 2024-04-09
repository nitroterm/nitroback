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
    [HttpGet("/api/nitroterm/v1/user/{username}")]
    [Authorize]
    public object Get(string username)
    {
        using NitrotermDbContext db = new();

        User? dbUser = db.Users
            .Include(user => user.Product)
            .FirstOrDefault(user => user.Username == username);
        
        if (dbUser == null) return NotFound(new ErrorResultDto("not_found", "user not found"));

        return new ResultDto<UserDto?>(new UserDto(dbUser));
    }

    [HttpPut("/api/nitroterm/v1/user")]
    [Authorize]
    public object Put([FromBody] UserEditDto dto)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;

        if (!string.IsNullOrWhiteSpace(dto.DisplayName))
        {
            if (db.Users.Any(u => u.DisplayName == dto.DisplayName && u.Id != user.Id))
                return BadRequest(new ErrorResultDto("already_taken", "username is already taken"));
            
            if (!Utilities.Utilities.CheckDisplayName(dto.DisplayName))
                return BadRequest(new ErrorResultDto("format_error", "invalid display name format"));
            
            user.DisplayName = dto.DisplayName;
        }

        if (!string.IsNullOrWhiteSpace(dto.Bio))
        {
            if (dto.Bio!.Length > 4000)
            {
                return BadRequest(new ErrorResultDto("too_large",
                    "bio length must be less than 4000 characters"));
            }
            
            user.Bio = dto.Bio;
        }
        if (dto.ProductId != null) user.Product = db.Products.Find(dto.ProductId!);

        db.Update(user);
        db.SaveChanges();

        return new ResultDto<UserDto?>(new UserDto(user));
    }
}