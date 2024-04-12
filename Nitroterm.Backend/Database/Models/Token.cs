namespace Nitroterm.Backend.Database.Models;

public class Token
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Value { get; set; }
    public TokenType Type { get; set; }
    public string? Device { get; set; }
}

public enum TokenType
{
    AuthJwt,
    Firebase
}