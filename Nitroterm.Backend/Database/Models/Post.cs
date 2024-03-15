namespace Nitroterm.Backend.Database.Models;

public class Post
{
    public Guid PublicIdentifier { get; set; }
    public int Id { get; set; }
    public User Sender { get; set; }
    public string Message { get; set; }
    public int NitroLevel { get; set; }
}