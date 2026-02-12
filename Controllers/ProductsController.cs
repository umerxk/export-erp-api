using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExportERP.Api.Data;
using ExportERP.Api.Entities;

namespace ExportERP.Api.Controllers;


[ApiController]
[Route("api/products")]

public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(product);
    }
}
