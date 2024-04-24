namespace Nitroterm.Backend.Database.Models;

public class Asset
{
    public int Id { get; set; }
    
    public int Width { get; set; }
    public int Height { get; set; }
    public string Format { get; set; }
    public byte[] Data { get; set; }

    public Asset()
    {
        
    }
}