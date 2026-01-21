# Alza E-Shop API

A modern e-commerce REST API built with .NET 8.0 following Clean Architecture principles. This API provides comprehensive product management capabilities with built-in API versioning, automatic database migrations, and extensive API documentation.

## 📋 Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Running Tests](#running-tests)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Configuration](#configuration)

## 🎯 Overview

Alza.EShop is a production-ready e-commerce API that demonstrates best practices in modern .NET development. The application is structured using Clean Architecture principles, ensuring separation of concerns, maintainability, and testability.

## ✨ Key Features

- **Clean Architecture** - Clear separation between Domain, Application, Infrastructure, and Presentation layers
- **RESTful API** - Fully compliant REST endpoints with proper HTTP verbs and status codes
- **API Versioning** - Support for multiple API versions (v1, v2) with URL-based versioning
- **Swagger/OpenAPI** - Interactive API documentation with detailed endpoint descriptions
- **Database Migrations** - Automatic EF Core migrations applied on startup
- **Data Seeding** - Automatic initial data population for development
- **Request Validation** - FluentValidation for comprehensive input validation
- **Structured Logging** - Serilog integration with file and console output
- **Global Exception Handling** - Centralized error handling middleware
- **CORS Support** - Configured for cross-origin resource sharing
- **Object Mapping** - AutoMapper for DTO transformations
- **Unit & Integration Tests** - Comprehensive test coverage with xUnit
- **In-Memory Testing** - Fast test execution using in-memory database

## 🏗️ Architecture

The solution follows Clean Architecture with the following layers:

```
Alza.EShop/
├── Alza.EShop.API/              # Presentation Layer
│   ├── Controllers/              # API Controllers (v1, v2)
│   ├── Middleware/               # Custom middleware
│   ├── MappingProfiles/          # AutoMapper profiles
│   └── Swagger/                  # Swagger configuration
├── Alza.EShop.Application/       # Application Layer
│   ├── Services/                 # Business logic services
│   └── Validators/               # FluentValidation validators
├── Alza.EShop.Domain/            # Domain Layer
│   ├── Entities/                 # Domain entities
│   ├── Interfaces/               # Repository interfaces
│   └── Common/                   # Base entities
├── Alza.EShop.Infrastructure/    # Infrastructure Layer
│   ├── Data/                     # DbContext, Configurations, Seeding
│   ├── Migrations/               # EF Core migrations
│   └── Repositories/             # Repository implementations
└── Alza.EShop.Tests/             # Test Layer
    └── Integration/              # Integration tests
```

## 🛠️ Technologies

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database access
- **Azure SQL Server** - Cloud database service
- **Swagger/OpenAPI** - API documentation
- **FluentValidation** - Request validation library
- **Serilog** - Structured logging framework
- **AutoMapper** - Object-to-object mapping
- **xUnit** - Testing framework
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing support

## 📦 Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 8.0 SDK** or later
  - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify installation: `dotnet --version`

- **Azure SQL Database** or **SQL Server**
  - Azure SQL Database (configured by default)
  - Or SQL Server 2019+ / LocalDB for local development

- **Git** (for cloning the repository)
  - Download from: https://git-scm.com/downloads

- **Visual Studio 2022** (optional, recommended)
  - Or Visual Studio Code with C# extension
  - Or any preferred code editor

## 🚀 Getting Started

Follow these steps to get the application running on your local machine:

### 1. Clone the Repository

```cmd
git clone <repository-url>
cd Alza
```

### 2. Configure Database Connection

The application uses Azure SQL Server by default. The connection string is located in [`appsettings.json`](Alza.EShop.API/appsettings.json).

**Option A: Use Existing Azure Database (Default)**

The default connection string is pre-configured. No changes needed.

**Option B: Use Local SQL Server**

Edit [`appsettings.json`](Alza.EShop.API/appsettings.json) and [`appsettings.Development.json`](Alza.EShop.API/appsettings.Development.json):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AlzaEShop;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. Restore Dependencies

Navigate to the solution directory and restore NuGet packages:

```cmd
dotnet restore
```

### 4. Build the Solution

Build the entire solution to ensure everything compiles correctly:

```cmd
dotnet build
```

### 5. Run Database Migrations

The application automatically applies migrations on startup, but you can manually run them:

```cmd
cd Alza.EShop.API
dotnet ef database update
```

**Note:** Database migrations are automatically applied when the application starts, so this step is optional.

### 6. Run the Application

Start the API server:

```cmd
cd Alza.EShop.API
dotnet run
```

The API will start and display the URLs it's listening on:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 7. Access Swagger UI

Once the application is running, open your web browser and navigate to:

```
https://localhost:7001/swagger
```

Or:

```
http://localhost:5000/swagger
```

The Swagger UI provides:
- Interactive API documentation
- Ability to test endpoints directly from the browser
- Request/response schemas
- Example request payloads
- Multiple API version support (v1, v2)

### 8. Test the API

You can test the API using:

**Using Swagger UI:**
1. Navigate to https://localhost:7001/swagger
2. Select an API version (V1 or V2)
3. Expand any endpoint (e.g., GET /api/v1/products)
4. Click "Try it out"
5. Click "Execute"

**Using curl:**

```cmd
curl https://localhost:7001/api/v1/products
```

**Using PowerShell:**

```powershell
Invoke-RestMethod -Uri "https://localhost:7001/api/v1/products" -Method Get
```

### 9. View Logs

Logs are written to both console and file. Development logs are stored in:

```
Alza.EShop.API/logs/alza-<date>.txt
```

## 🧪 Running Tests

The solution includes comprehensive unit and integration tests using xUnit.

### Run All Tests

From the solution root directory:

```cmd
dotnet test
```

### Run Tests with Detailed Output

```cmd
dotnet test --verbosity normal
```

### Run Tests for Specific Project

```cmd
dotnet test Alza.EShop.Tests\Alza.EShop.Tests.csproj
```

### View Test Results

The test runner will display results in the console:

```
Passed!  - Failed:     0, Passed:    10, Skipped:     0, Total:    10
```

### Test Coverage

The test project includes:
- **Integration Tests** - Full API endpoint testing with in-memory database
- **Service Tests** - Business logic validation
- **Repository Tests** - Data access layer verification

**Test Location:** [`Alza.EShop.Tests/Integration/ProductsApiTests.cs`](Alza.EShop.Tests/Integration/ProductsApiTests.cs)

### Running Specific Tests

To run a specific test class:

```cmd
dotnet test --filter "FullyQualifiedName~ProductsApiTests"
```

To run tests by trait or category (if configured):

```cmd
dotnet test --filter "Category=Integration"
```

## 📚 API Documentation

### Available Endpoints (v1)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/products` | Get all products |
| GET | `/api/v1/products/{id}` | Get product by ID |
| POST | `/api/v1/products` | Create new product |
| PATCH | `/api/v1/products/{id}/stock` | Update product stock |

### Sample Request - Create Product

```bash
POST https://localhost:7001/api/v1/products
Content-Type: application/json

{
  "name": "Wireless Mouse",
  "imageUrl": "https://example.com/images/mouse.jpg",
  "price": 29.99,
  "description": "Ergonomic wireless mouse with precision tracking",
  "stockQuantity": 100
}
```

### Sample Response

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Wireless Mouse",
  "imageUrl": "https://example.com/images/mouse.jpg",
  "price": 29.99,
  "description": "Ergonomic wireless mouse with precision tracking",
  "stockQuantity": 100
}
```

### API Versioning

The API supports versioning through the URL:
- **v1:** `/api/v1/products`
- **v2:** `/api/v2/products`

Select the version in Swagger UI from the top-right dropdown.

## 📁 Project Structure

### Alza.EShop.API
Web API layer containing controllers, middleware, and API configuration. Entry point of the application.

**Key Files:**
- [`Program.cs`](Alza.EShop.API/Program.cs) - Application startup and configuration
- [`Controllers/V1/ProductsController.cs`](Alza.EShop.API/Controllers/V1/ProductsController.cs) - Product endpoints v1
- [`Middleware/ExceptionHandlingMiddleware.cs`](Alza.EShop.API/Middleware/ExceptionHandlingMiddleware.cs) - Global error handling

### Alza.EShop.Application
Application services layer containing business logic, DTOs, and validators.

**Key Files:**
- [`Services/Implementations/ProductService.cs`](Alza.EShop.Application/Services/Implementations/ProductService.cs) - Product business logic
- [`Validators/CreateProductRequestValidator.cs`](Alza.EShop.Application/Validators/CreateProductRequestValidator.cs) - Input validation

### Alza.EShop.Domain
Domain layer containing entities and repository interfaces. Core business model.

**Key Files:**
- [`Entities/Product.cs`](Alza.EShop.Domain/Entities/Product.cs) - Product entity
- [`Interfaces/IProductRepository.cs`](Alza.EShop.Domain/Interfaces/IProductRepository.cs) - Repository contract

### Alza.EShop.Infrastructure
Infrastructure layer containing data access, EF Core configurations, and migrations.

**Key Files:**
- [`Data/EShopDbContext.cs`](Alza.EShop.Infrastructure/Data/EShopDbContext.cs) - Entity Framework DbContext
- [`Data/DbSeeder.cs`](Alza.EShop.Infrastructure/Data/DbSeeder.cs) - Initial data seeding
- [`Repositories/ProductRepository.cs`](Alza.EShop.Infrastructure/Repositories/ProductRepository.cs) - Data access implementation

### Alza.EShop.Tests
Test project containing unit and integration tests.

**Key Files:**
- [`Integration/ProductsApiTests.cs`](Alza.EShop.Tests/Integration/ProductsApiTests.cs) - API integration tests
- [`InMemoryDbContextFactory.cs`](Alza.EShop.Tests/InMemoryDbContextFactory.cs) - Test database setup

## ⚙️ Configuration

### Application Settings

Configuration is managed through:
- [`appsettings.json`](Alza.EShop.API/appsettings.json) - Production settings
- [`appsettings.Development.json`](Alza.EShop.API/appsettings.Development.json) - Development settings

### Key Configuration Sections

**Connection Strings:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=...;User ID=...;Password=..."
}
```

**Logging (Development):**
```json
"Serilog": {
  "MinimumLevel": "Debug",
  "WriteTo": [
    { "Name": "Console" },
    { 
      "Name": "File",
      "Args": {
        "path": "logs/alza-.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

### Environment Variables

You can override settings using environment variables:

```cmd
set ConnectionStrings__DefaultConnection=Server=localhost;Database=AlzaEShop;...
dotnet run
```

## 🔒 Security Notes

**Important:** The connection string in the repository contains credentials for demonstration purposes only.

In production:
- Store connection strings in **Azure Key Vault**
- Use **Managed Identities** for Azure resources
- Enable **environment variables** for sensitive data
- Never commit credentials to source control

## 🤝 Contributing

1. Follow Clean Architecture principles
2. Write tests for new features
3. Use FluentValidation for input validation
4. Document API endpoints with XML comments
5. Follow existing code style and conventions

## 📄 License

This project is created for demonstration and educational purposes.

## 📞 Support

For issues and questions:
- Review the API documentation at `/swagger`
- Check the logs in `Alza.EShop.API/logs/`
- Ensure database connectivity
- Verify .NET 8.0 SDK installation

---

**Happy Coding! 🚀**
