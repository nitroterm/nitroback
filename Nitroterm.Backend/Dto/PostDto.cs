using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Dto;

public class PostDto(Post post)
{
    public Guid Id { get; set; } = post.PublicIdentifier;
    public UserDto Sender { get; set; } = new(post.Sender);
    public string Message { get; set; } = post.Message;
    public int NitroLevel { get; set; } = post.NitroLevel;
    public bool Edited { get; set; } = post.Edited;
    public DateTime CreationDate { get; set; } = post.CreationTimestamp;
    public DateTime? EditionDate { get; set; } = post.LastEditionTimestamp;

    public PostStatsDto Stats
    {
        get
        {
            using NitrotermDbContext db = new();
            UserToPostInteraction[] interactions = db.GetInteractionsForPost(post);

            return new PostStatsDto()
            {
                NitroCount = interactions.Count(interaction => interaction.Type == UserToPostInteractionType.Nitro),
                DynamiteCount = interactions.Count(interaction => interaction.Type == UserToPostInteractionType.Dynamite),
                ForkCount = 0 // TODO
            };
        }
    }
}