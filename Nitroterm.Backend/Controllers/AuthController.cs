﻿using Microsoft.AspNetCore.Mvc;
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
    /// <summary>
    /// Validate a user authentication token
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<UserDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [HttpGet("validate")]
    [Authorize]
    public object Validate()
    {
        User? user = this.GetUser()!;
        
        return Ok(new ResultDto<UserDto?>(new UserDto(user)));
    }
    
    /// <summary>
    /// Register an account
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<LoginResponseDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("register")]
    public object Register([FromBody] LoginDto dto)
    {
        using NitrotermDbContext db = new();

        if (dto.ReCaptchaChallenge == null || !ReCaptcha.Verify(dto.ReCaptchaChallenge))
        {
            return BadRequest(new ErrorResultDto("recaptcha_error", "recaptcha check failed"));
        }

        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new ErrorResultDto("format_error", "username or/and password are empty"));
        
        if (!Utilities.Utilities.CheckUsername(dto.Username))
            return BadRequest(new ErrorResultDto("format_error", "invalid username format : username" +
                                                                 " must be lowercase, must be less than 30 characters" +
                                                                 " long, and can only contains alphanumeric characters" +
                                                                 " or underscores"));
        if (!Utilities.Utilities.CheckPassword(dto.Password))
            return BadRequest(new ErrorResultDto("format_error", "invalid password format : password " +
                                                                 "must be at least 5 characters long and contains " +
                                                                 "letters and numbers"));
        if (dto.Password.Length < 5)
            return BadRequest(new ErrorResultDto("format_error", "password must have at least 5 characters"));

        User? existingUser = db.Users.FirstOrDefault(user => user.Username == dto.Username);
        if (existingUser != null) return BadRequest(new ErrorResultDto("already_exists", "user already exists"));

        User user = db.CreateUser(dto.Username, dto.Password);
        string token = user.IssueJwtToken(db);
        user.LatestLoginTimestamp = DateTime.Now;
        
        if (dto.FirebaseToken != null && !db.Tokens.Any(token => token.Value == dto.FirebaseToken))
            db.AddToken(user, TokenType.Firebase, dto.FirebaseToken);
        
        db.Update(user);
        db.SaveChanges();

        return Ok(new ResultDto<LoginResponseDto?>(new LoginResponseDto(new UserDto(user), token)));
    }
    
    /// <summary>
    /// Login to an account
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(typeof(ResultDto<LoginResponseDto?>), 200)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("login")]
    public object Login([FromBody] LoginDto dto)
    {
        using NitrotermDbContext db = new();

        User? existingUser = db.GetUser(dto.Username, dto.Password);
        if (existingUser == null) return BadRequest(new ErrorResultDto("user_login_error", "user does not exist or password is invalid"));

        string token = existingUser.IssueJwtToken(db);
        existingUser.LatestLoginTimestamp = DateTime.Now;
        
        if (dto.FirebaseToken != null && !db.Tokens.Any(token => token.Value == dto.FirebaseToken))
            db.AddToken(existingUser, TokenType.Firebase, dto.FirebaseToken);
        
        db.Update(existingUser);
        db.SaveChanges();

        return Ok(new ResultDto<LoginResponseDto?>(new LoginResponseDto(new UserDto(existingUser), token)));
    }
    
    /// <summary>
    /// Change the current user's password
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("password")]
    [Authorize]
    public object ChangePassword([FromBody] ChangePasswordDto dto)
    {
        using NitrotermDbContext db = new();

        User? existingUser = this.GetUser();
        if (existingUser == null || !existingUser.CheckPassword(dto.Current)) 
            return BadRequest(new ErrorResultDto("user_login_error", "user does not exist or password is invalid"));
        
        if (!Utilities.Utilities.CheckPassword(dto.New))
            return BadRequest(new ErrorResultDto("format_error", "invalid password format : password " +
                                                                 "must be at least 5 characters long and contains " +
                                                                 "letters and numbers"));
        
        existingUser.SetPassword(dto.New);
        db.Update(existingUser);
        db.ExpireUserTokens(existingUser);

        return Ok();
    }
    
    /// <summary>
    /// Logout from the current account
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ErrorResultDto), 401)]
    [ProducesResponseType(typeof(ErrorResultDto), 400)]
    [HttpPost("logout")]
    [Authorize]
    public object Logout()
    {
        using NitrotermDbContext db = new();

        User? existingUser = this.GetUser();
        if (existingUser == null) return BadRequest(new ErrorResultDto("user_login_error", "user does not exist"));
        
        db.ExpireUserTokens(existingUser);

        return Ok();
    }
}