# Export ERP API

A .NET 10.0 Web API for the Export ERP system, providing product management endpoints.

## Features

- RESTful API with ASP.NET Core
- Entity Framework Core with SQLite database
- Swagger/OpenAPI documentation
- CORS enabled for frontend integration

## Prerequisites

- .NET 10.0 SDK or later

## Getting Started

1. Restore dependencies:
   ```bash
   dotnet restore
   ```

2. Run database migrations (if applicable):
   ```bash
   dotnet ef database update
   ```

3. Run the API:
   ```bash
   dotnet run
   ```

4. Access Swagger UI at: `http://localhost:5000/swagger` (or the port shown in console)

## API Endpoints

- `GET /api/products` - Get all products
- `POST /api/products` - Create a new product

## Database

The application uses SQLite with the database file `app.db` in the project root.

## Project Structure

- `Controllers/` - API controllers
- `Data/` - Database context and configuration
- `Entities/` - Entity models
