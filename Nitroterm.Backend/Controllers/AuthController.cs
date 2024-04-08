using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/auth")]
public class AuthController : ControllerBase
{
    [HttpGet("validate")]
    [Authorize]
    public object Validate()
    {
        User? user = this.GetUser()!;
        
        return Ok(new ResultDto<UserDto>(new UserDto(user)));
    }
    
    [HttpPost("register")]
    public object Register([FromBody] LoginDto dto)
    {
        using NitrotermDbContext db = new();

        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new ErrorResultDto("format_error", "username or/and password are empty"));

        User? existingUser = db.Users.FirstOrDefault(user => user.Username == dto.Username);
        if (existingUser != null) return BadRequest(new ErrorResultDto("already_exists", "user already exists"));

        User user = db.CreateUser(dto.Username, dto.Password);
        string token = user.IssueJwtToken(db);
        db.Update(user);
        db.SaveChanges();

        return Ok(new ResultDto<LoginResponseDto>(new LoginResponseDto(new UserDto(user), token)));
    }
    
    [HttpPost("login")]
    public object Login([FromBody] LoginDto dto)
    {
        using NitrotermDbContext db = new();

        User? existingUser = db.GetUser(dto.Username, dto.Password);
        if (existingUser == null) return BadRequest(new ErrorResultDto("user_login_error", "user does not exist or password is invalid"));

        string token = existingUser.IssueJwtToken(db);
        db.Update(existingUser);
        db.SaveChanges();

        return Ok(new ResultDto<LoginResponseDto>(new LoginResponseDto(new UserDto(existingUser), token)));
    }
    
    [HttpPost("password")]
    [Authorize]
    public object ChangePassword([FromBody] ChangePasswordDto dto)
    {
        using NitrotermDbContext db = new();

        User? existingUser = this.GetUser();
        if (existingUser == null || !existingUser.CheckPassword(dto.Current)) 
            return BadRequest(new ErrorResultDto("user_login_error", "user does not exist or password is invalid"));
        
        existingUser.SetPassword(dto.New);
        db.ExpireUserTokens(existingUser);

        db.Update(existingUser);
        db.SaveChanges();

        return Ok();
    }
    
    [HttpPost("logout")]
    [Authorize]
    public object Logout()
    {
        using NitrotermDbContext db = new();

        User? existingUser = this.GetUser();
        if (existingUser == null) return BadRequest(new ErrorResultDto("user_login_error", "user does not exist"));
        
        db.ExpireUserTokens(existingUser);

        db.Update(existingUser);
        db.SaveChanges();

        return Ok(new ResultDto<string>("ok"));
    }
}