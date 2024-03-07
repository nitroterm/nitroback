using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Dto;

public class ProductDto(Product product)
{
    public int Id { get; set; } = product.Id;
    public string Slug { get; set; } = product.Slug;
    public string Title { get; set; } = product.Title;
    public uint Color { get; set; } = product.Color;
}