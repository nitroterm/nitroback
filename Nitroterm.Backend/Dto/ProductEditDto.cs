using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Dto;

public class ProductEditDto
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public uint? Color { get; set; }
}