namespace Nitroterm.Backend.Database.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public Product Product { get; set; }
}