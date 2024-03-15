namespace Nitroterm.Backend.Database.Models;

public class Post
{
    public Guid PublicIdentifier { get; set; }
    public int Id { get; set; }
    public User Sender { get; set; }
    public string Message { get; set; }
    public int NitroLevel { get; set; }

    public PostUserInteractionType? GetUserInteraction(NitrotermDbContext db, User user)
        => db.GetInteractionsForPost(user, this)?.Type;
    public bool IsNitronizedByUser(NitrotermDbContext db, User user)
        => GetUserInteraction(db, user) == PostUserInteractionType.Nitronize;
    public bool IsDynamitedByUser(NitrotermDbContext db, User user)
        => GetUserInteraction(db, user) == PostUserInteractionType.Dynamite;
}