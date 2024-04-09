using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/products")]
public class ProductsController : ControllerBase
{
    [HttpPost("/api/nitroterm/v1/products")]
    [Authorize]
    public object Create([FromBody] ProductEditDto dto)
    {
        using NitrotermDbContext db = new();
        User user = this.GetUser()!;
        if (user.Level < UserExecutionLevel.Administrator) return NotFound();

        if (!Utilities.Utilities.CheckUsername(dto.Slug))
            return NotFound(new ErrorResultDto("format_error", "product slug format error : must be lowercase"));

        if (!Utilities.Utilities.CheckUserContent(dto.Title))
            return NotFound(new ErrorResultDto("format_error", "product name format error : must be less than 4000 characters"));
        
        if (db.Products.Any(product => product.Slug == dto.Slug)) 
            return NotFound(new ErrorResultDto("already_exists", "product already exists (slug)"));
        if (db.Products.Any(product => product.Title == dto.Title)) 
            return NotFound(new ErrorResultDto("not_found", "product already exists (title)"));

        Product product = new()
        {
            Slug = dto.Slug,
            Title = dto.Title,
            Color = dto.Color ?? 0xFF000000
        };

        db.Products.Add(product);
        db.SaveChanges();

        return new ResultDto<ProductDto?>(new ProductDto(product));
    }
    
    [HttpGet("/api/nitroterm/v1/product/{id:int}")]
    [Authorize]
    public object Get(int id)
    {
        using NitrotermDbContext db = new();

        Product? product = db.Products.Find(id);
        if (product == null) return NotFound(new ErrorResultDto("not_found", "product not found"));

        return new ResultDto<ProductDto?>(new ProductDto(product));
    }

    [HttpGet]
    [Authorize]
    public object GetProducts()
    {
        using NitrotermDbContext db = new();

        Product[] products = db.Products.Take(10).ToArray();

        return new ResultDto<Product[]?>(products);
    }

    [HttpGet("/api/nitroterm/v1/products/query/{input}")]
    [Authorize]
    public object QueryProducts(string input)
    {
        using NitrotermDbContext db = new();

        Product[] products = db.Products.Where(product => product.Title.ToLower().Contains(input.ToLower().Trim()))
            .Take(10).ToArray();

        return new ResultDto<Product[]?>(products);
    }
}