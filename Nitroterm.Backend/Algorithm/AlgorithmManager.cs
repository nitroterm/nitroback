using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;

namespace Nitroterm.Backend.Algorithm;

public static class AlgorithmManager
{
    public static int DeduceInitialNitroLevelForPost(Post post, User sender)
    {
        return sender.NitroLevel;
    }

    public static Post[] DeduceBestPostsForUser(User? user, int count)
    {
        using NitrotermDbContext db = new();
        
        return db.Posts
            .Include(post => post.Sender)
            .Include(post => post.Sender.Product)
            .Where(post => post == null || post.Sender != user)
            .Take(count)
            .OrderByDescending(post => post.CreationTimestamp)
            .ThenByDescending(post => post.NitroLevel)
            .ToArray();
    }

    public static Post[] DeduceBestPostsFromUser(User user, int count)
    {
        using NitrotermDbContext db = new();

        return db.Posts
            .Include(post => post.Sender)
            .Where(post => post.Sender == user)
            .Take(count)
            .OrderByDescending(post => post.NitroLevel)
            .ToArray();
    }
}