using Microsoft.EntityFrameworkCore;
using ExportERP.Api.Entities;

namespace ExportERP.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
