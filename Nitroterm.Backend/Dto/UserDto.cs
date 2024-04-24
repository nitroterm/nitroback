using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Dto;

public class UserDto(User user)
{
    public string Username { get; } = user.Username;
    public string? DisplayName { get; } = user.DisplayName;
    public string? Bio { get; } = user.Bio;
    public ProductDto? Product { get; } = user.Product == null ? null : new ProductDto(user.Product);
    public string ProfilePicture { get; } = user.ProfilePictureUrl;
}