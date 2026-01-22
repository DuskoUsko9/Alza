using Alza.EShop.API.Middleware;
using Alza.EShop.API.Swagger;
using Alza.EShop.Application.Services.Implementations;
using Alza.EShop.Application.Services.Interfaces;
using Alza.EShop.Application.Validators;
using Alza.EShop.Domain.Interfaces;
using Alza.EShop.Infrastructure.Data;
using Alza.EShop.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// API Versioning configuration
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Add API Explorer for automatic Swagger version discovery
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";  // Format: v1, v2, v3
    options.SubstituteApiVersionInUrl = true;  // Replace {version:apiVersion} in routes
});

// Add a transient service to configure Swagger options
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Swagger configuration with XML documentation
builder.Services.AddSwaggerGen(options =>
{
    // Enable XML documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Database configuration - Using Azure SQL Server 
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EShopDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository registration
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Service registration
builder.Services.AddScoped<IProductService, ProductService>();

// AutoMapper registration
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// FluentValidation registration
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EShopDbContext>();
    
    // Apply any pending migrations only for relational databases
    // InMemory databases (used in tests) don't support migrations
    if (context.Database.IsRelational())
    {
        await context.Database.MigrateAsync();
    }
    else
    {
        // For InMemory databases, ensure the database is created
        await context.Database.EnsureCreatedAsync();
    }
    
    // Seed database if empty
    DbSeeder.SeedData(context);
}

// Configure the HTTP request pipeline

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    
    // Build a swagger endpoint for each discovered API version
    foreach (var description in provider.ApiVersionDescriptions)
    {
        var url = $"/swagger/{description.GroupName}/swagger.json";
        var name = description.GroupName.ToUpperInvariant();
        options.SwaggerEndpoint(url, name);
    }
    
    options.DocumentTitle = "Alza API Documentation";
});

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
