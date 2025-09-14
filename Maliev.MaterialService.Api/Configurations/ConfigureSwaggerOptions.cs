using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Maliev.MaterialService.Api.Configurations;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly SwaggerOptions _swaggerOptions;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<SwaggerOptions> swaggerOptions)
    {
        _provider = provider;
        _swaggerOptions = swaggerOptions.Value;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        // Add JWT Bearer authorization
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = _swaggerOptions.Title,
            Version = description.ApiVersion.ToString(),
            Description = _swaggerOptions.Description,
            Contact = new OpenApiContact
            {
                Name = _swaggerOptions.Contact.Name,
                Email = _swaggerOptions.Contact.Email,
                Url = !string.IsNullOrEmpty(_swaggerOptions.Contact.Url) ? new Uri(_swaggerOptions.Contact.Url) : null
            },
            License = new OpenApiLicense
            {
                Name = _swaggerOptions.License.Name,
                Url = !string.IsNullOrEmpty(_swaggerOptions.License.Url) ? new Uri(_swaggerOptions.License.Url) : null
            }
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}