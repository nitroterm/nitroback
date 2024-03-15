using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Dto;

public class PostDto(Post post)
{
    public Guid Id { get; set; } = post.PublicIdentifier;
    public UserDto Sender { get; set; } = new(post.Sender);
    public string Message { get; set; } = post.Message;
    public int NitroLevel { get; set; } = post.NitroLevel;
}