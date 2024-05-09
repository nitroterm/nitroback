using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Database;

public class NitrotermDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<UserToUserInteraction> UserToUserInteractions { get; set; }
    public DbSet<UserToPostInteraction> UserToPostInteractions { get; set; }

    public NitrotermDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(Secrets.Instance.ConnectionString, new MariaDbServerVersion("11.3.2"));
    }

    public User? GetUser(int id)
        => Users
            .Include(user => user.Product)
            .Include(user => user.Tokens)
            .Include(user => user.ProfilePicture)
            .FirstOrDefault(user => user.Id == id);

    public User? GetUser(string username)
        => Users
            .Include(user => user.Product)
            .Include(user => user.Tokens)
            .Include(user => user.ProfilePicture)
            .FirstOrDefault(user => user.Username == username);

    public Post? GetPost(Guid publicId)
        => Posts
            .Include(post => post.Sender)
            .FirstOrDefault(user => user.PublicIdentifier == publicId);

    public Post? GetPost(int id)
        => Posts
            .Include(post => post.Sender)
            .FirstOrDefault(user => user.Id == id);

    public UserToUserInteraction[] GetInteractionsForUser(User targetUser)
        => UserToUserInteractions
            .Include(pui => pui.SourceUser)
            .Include(pui => pui.TargetUser)
            .Where(pui => pui.TargetUser == targetUser)
            .ToArray();

    public UserToPostInteraction[] GetInteractionsForPost(User sourceUser, Post post)
        => UserToPostInteractions
            .Include(userToPost => userToPost.SourceUser)
            .Include(userToPost => userToPost.Post)
            .Where(userToPost => userToPost.SourceUser == sourceUser && userToPost.Post == post)
            .ToArray();

    public UserToPostInteraction[] GetInteractionsForPost(Post post)
        => UserToPostInteractions
            .Include(userToPost => userToPost.SourceUser)
            .Include(userToPost => userToPost.Post)
            .Where(userToPost => userToPost.Post == post)
            .ToArray();

    public UserToUserInteraction[] GetInteractionsForUser(User sourceUser, User targetUser)
        => UserToUserInteractions
            .Include(userToUser => userToUser.SourceUser)
            .Include(userToUser => userToUser.TargetUser)
            .Where(userToUser => userToUser.SourceUser == sourceUser && userToUser.TargetUser == targetUser)
            .ToArray();

    public bool ClearPostInteractions(User sourceUser, Post post)
        => InteractWithPost(sourceUser, post, UserToPostInteractionType.None);

    public bool InteractWithPost(User sourceUser, Post post, UserToPostInteractionType type)
    {
        UserToPostInteraction[] pastOtherInteractions = GetInteractionsForPost(sourceUser, post).ToArray();
        if (pastOtherInteractions.Any(interaction => interaction.Type == type))
            return false;

        if (type != UserToPostInteractionType.None)
        {
            UserToPostInteraction interaction = pastOtherInteractions.FirstOrDefault() ?? new UserToPostInteraction();
            interaction.Post = post;
            interaction.SourceUser = sourceUser;
            interaction.Type = type;

            UserToPostInteractions.Update(interaction);
        }

        SaveChanges();

        return true;
    }

    public bool ClearUserInteractions(User sourceUser, User targetUser)
        => InteractWithUser(sourceUser, targetUser, UserToUserInteractionType.None);

    public bool InteractWithUser(User sourceUser, User targetUser, UserToUserInteractionType type)
    {
        UserToUserInteraction[] pastOtherInteractions = GetInteractionsForUser(sourceUser, targetUser)
            .Where(interaction => interaction.Type != type)
            .ToArray();
        if (pastOtherInteractions.Any())
            UserToUserInteractions.RemoveRange(pastOtherInteractions);

        if (type != UserToUserInteractionType.None)
        {
            UserToUserInteraction interaction = new UserToUserInteraction()
            {
                SourceUser = sourceUser,
                TargetUser = targetUser,
                Type = type
            };
            UserToUserInteractions.Update(interaction);
        }

        SaveChanges();

        return true;
    }

    public User? GetUser(string username, string password)
    {
        User? user = GetUser(username);
        if (user == null) return null;

        return user.CheckPassword(password) ? user : null;
    }

    public User CreateUser(string username, string password)
    {
        User user = new()
        {
            Username = username.Trim(),
            CreationTimestamp = DateTime.Now,
            LatestLoginTimestamp = DateTime.Now,
            Level = UserExecutionLevel.User
        };

        user.SetPassword(password.Trim());

        Users.Add(user);
        SaveChanges();

        return user;
    }

    public void ExpireUserTokens(User user)
    {
        Tokens.RemoveRange(user.Tokens.Where(token => token.Type == TokenType.AuthJwt));
        SaveChanges();
    }

    public void AddToken(User user, TokenType type, string value)
    {
        Tokens.Add(new Token {UserId = user.Id, Value = value, Type = type});
        SaveChanges();
    }
}