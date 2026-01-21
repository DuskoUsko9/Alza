using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Alza.EShop.API.Swagger;

/// <summary>
/// Configures Swagger generation options for API versioning.
/// </summary>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        // Create a Swagger document for each discovered API version
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Alza Product Management API",
            Version = description.ApiVersion.ToString(),
            Description = GetDescriptionForVersion(description.ApiVersion.MajorVersion ?? 1)
        };

        if (description.IsDeprecated)
        {
            info.Description += " **This API version has been deprecated.**";
        }

        return info;
    }

    private static string GetDescriptionForVersion(int majorVersion)
    {
        return majorVersion switch
        {
            1 => "REST API for product management with basic CRUD operations. " +
                 "Supports creating products, retrieving all products or by ID, and updating stock quantities synchronously.",
            2 => "Enhanced REST API for product management with pagination support for efficient product listing. " +
                 "Includes all v1 features plus paginated product listing.",
            _ => "Alza API"
        };
    }
}
