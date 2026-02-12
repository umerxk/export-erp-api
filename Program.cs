using Microsoft.EntityFrameworkCore;
using ExportERP.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add Controllers
builder.Services.AddControllers();

// 🔹 Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "PostgreSQL connection string is missing. Set ConnectionStrings:DefaultConnection.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔹 CORS (for React later)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Keep Swagger available in Azure for quick verification.
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection(); // keep off for now

app.UseCors("AllowFrontend");

app.MapGet("/", () => Results.Ok(new { status = "ok", service = "export-erp-api" }));

// 🔹 Enable Controllers
app.MapControllers();

app.Run();
