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
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback to SQLite for local development
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=app.db"));
}
else
{
    // Use SQL Server for Azure deployment
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString));
}

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

// 🔹 Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // keep off for now

app.UseCors("AllowFrontend");

// 🔹 Enable Controllers
app.MapControllers();

app.Run();
