namespace Nitroterm.Backend.Database.Models;

public class Product
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public uint Color { get; set; }
}