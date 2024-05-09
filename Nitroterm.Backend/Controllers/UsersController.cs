using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;
using SixLabors.ImageSharp;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/users")]
public class UsersController : ControllerBase
{
    private static byte[] _defaultProfilePictureBytes;
    
    [HttpGet("{username}")]
    public object Get(string username)
    {
        using NitrotermDbContext db = new();

        User? dbUser = db.Users
            .Include(user => user.Product)
            .FirstOrDefault(user => user.Username == username);
        
        if (dbUser == null) return NotFound(new ErrorResultDto("not_found", "user not found"));

        return new ResultDto<UserDto?>(new UserDto(dbUser));
    }
    
    [HttpPost("{username}/follow")]
    [Authorize]
    public object FollowUser(string username)
    {
        using NitrotermDbContext db = new();

        User? userToFollow = db.Users
            .Include(user => user.Product)
            .FirstOrDefault(user => user.Username == username);
        User sourceUser = this.GetUser()!;
        
        if (userToFollow == null) return NotFound(new ErrorResultDto("not_found", "user not found"));

        db.InteractWithUser(sourceUser, userToFollow, UserToUserInteractionType.Follow);

        return new ResultDto<UserDto?>(new UserDto(userToFollow));
    }
    
    [HttpGet("{username}/picture")]
    public object GetUserPicture(string username)
    {
        using NitrotermDbContext db = new();

        User? dbUser = db.Users
            .Include(user => user.ProfilePicture)
            .FirstOrDefault(user => user.Username == username);

        if (dbUser == null) return NotFound();
        if (dbUser.ProfilePicture == null)
        {
            if (_defaultProfilePictureBytes == null)
                _defaultProfilePictureBytes = System.IO.File.ReadAllBytes("Resources/default_profile_picture.png");

            return File(_defaultProfilePictureBytes, "image/png");
        }

        return File(dbUser.ProfilePicture!.Data, $"image/{dbUser.ProfilePicture.Format}");
    }
    
    [HttpGet("/api/nitroterm/v1/user")]
    [Authorize]
    public object GetLoginUser()
    {
        using NitrotermDbContext db = new();

        return new ResultDto<UserDto?>(new UserDto(this.GetUser()!));
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
    
    [HttpGet("/api/nitroterm/v1/user/picture")]
    [Authorize]
    public object GetConnectedUserPicture()
    {
        using NitrotermDbContext db = new();

        User user = this.GetUser()!;

        if (user.ProfilePicture == null)
        {
            if (_defaultProfilePictureBytes == null)
                _defaultProfilePictureBytes = System.IO.File.ReadAllBytes("Resources/default_profile_picture.png");

            return File(_defaultProfilePictureBytes, "image/png");
        }

        return File(user.ProfilePicture!.Data, $"image/{user.ProfilePicture.Format}");
    }

    [HttpPost("/api/nitroterm/v1/user/picture")]
    [Authorize]
    public object EditProfilePicture(IFormFile file)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        
        Stream fileStream = file.OpenReadStream();
        byte[] data = new byte[fileStream.Length];

        int newSize = fileStream.Read(data, 0, data.Length);
        Array.Resize(ref data, newSize);

        Image? image;
        try
        {
            image = Image.Load(data);
            if (image == null) return BadRequest(new ErrorResultDto("bad_format", "bad image format"));
            
            if (image.Width > 1024 || image.Height > 1024)
                return BadRequest(new ErrorResultDto("bad_format", "image must be in 1024x1024 maximum"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(new ErrorResultDto("bad_format", "bad image format"));
        }

        Asset asset = user.ProfilePicture ?? new Asset
        {
            Format = Path.GetExtension(file.FileName).TrimStart('.'),
            Data = data,
            Width = image.Width,
            Height = image.Height
        };

        user.ProfilePicture = asset;

        db.Update(asset);
        db.Update(user);
        db.SaveChanges();
        
        image.Dispose();

        return new ResultDto<UserDto?>(new UserDto(user));
    }
}