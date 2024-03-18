using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Algorithm;

public static class AlgorithmManager
{
    public static int DeduceInitialNitroLevelForPost(Post post, User sender)
    {
        return sender.NitroLevel;
    }

    public static Post[] DeduceBestPostsForUser(User user, int count)
    {
        using NitrotermDbContext db = new();
        
        return db.Posts
            .Where(post => post.Sender != user)
            .Take(count)
            .OrderByDescending(post => post.NitroLevel)
            .ToArray();
    }
}