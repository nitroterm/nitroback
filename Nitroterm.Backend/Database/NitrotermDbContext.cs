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
    public DbSet<UserToUserInteraction> UserToUserInteractions { get; set; }
    public DbSet<Asset> Assets { get; set; }

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

    public UserToUserInteraction? GetInteractionsForUser(User owner, User user)
        => UserToUserInteractions
            .Include(pui => pui.User)
            .FirstOrDefault(pui => pui.Id == owner.Id && pui.User == user);

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