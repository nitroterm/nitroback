using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nitroterm.Backend.Attributes;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/products")]
public class ProductsController : ControllerBase
{
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