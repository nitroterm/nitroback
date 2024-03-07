namespace Nitroterm.Backend.Database.Models;

public class Token
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Value { get; set; }
}