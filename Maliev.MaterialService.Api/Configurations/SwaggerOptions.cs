namespace Maliev.MaterialService.Api.Configurations;

public class SwaggerOptions
{
    public const string SectionName = "Swagger";

    public string Title { get; set; } = "Maliev Material Service API";
    public string Description { get; set; } = "A comprehensive material management API for rapid prototyping and manufacturing operations.";
    public SwaggerContactOptions Contact { get; set; } = new();
    public SwaggerLicenseOptions License { get; set; } = new();
    public string RoutePrefix { get; set; } = "materials/swagger";
}

public class SwaggerContactOptions
{
    public string Name { get; set; } = "Maliev Co. Ltd.";
    public string Email { get; set; } = "api@maliev.com";
    public string Url { get; set; } = "https://maliev.com";
}

public class SwaggerLicenseOptions
{
    public string Name { get; set; } = "Proprietary";
    public string Url { get; set; } = "https://maliev.com/license";
}