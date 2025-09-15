using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.Configurations;

public class SwaggerOptions
{
    public const string SectionName = "Swagger";

    [Required]
    public string Title { get; set; } = "Maliev Material Service API";

    [Required]
    public string Description { get; set; } = "A comprehensive material management API for rapid prototyping and manufacturing operations.";

    [Required]
    public SwaggerContactOptions Contact { get; set; } = new();

    [Required]
    public SwaggerLicenseOptions License { get; set; } = new();

    [Required]
    public string RoutePrefix { get; set; } = "materials/swagger";
}

public class SwaggerContactOptions
{
    [Required]
    public string Name { get; set; } = "Maliev Co. Ltd.";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "api@maliev.com";

    [Required]
    [Url]
    public string Url { get; set; } = "https://maliev.com";
}

public class SwaggerLicenseOptions
{
    [Required]
    public string Name { get; set; } = "Proprietary";

    [Required]
    [Url]
    public string Url { get; set; } = "https://maliev.com/license";
}