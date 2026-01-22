# Alza API

## Technologies

- **.NET 8.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database access
- **Azure SQL Server** - Cloud database service
- **Swagger/OpenAPI** - API documentation
- **FluentValidation** - Request validation library
- **Serilog** - Structured logging framework
- **xUnit** - Testing framework

## Getting Started

Follow these steps to get the application running on your local machine:

### 1. Clone the Repository:

```cmd
git clone <repository-url>
```

### 2. Create user-secrets and run the application

```cmd
cd Alza.EShop.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:dinotechdb.database.windows.net,1433;Initial Catalog=dinotech;Persist Security Info=False;User ID=userid;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
dotnet user-secrets list
dotnet build
dotnet run
```

Application should listen on localhost:

```cmd
http://localhost:5079
https://localhost:7090
```

The application automatically applies migrations on startup, but you can manually run them:
```cmd
cd Alza.EShop.API
dotnet ef database update
```

## Test the API

You can test the API **using Swagger UI:**
http://localhost:5079/swagger/index.html

The solution also includes comprehensive unit and integration tests using xUnit:

```cmd
dotnet test
```