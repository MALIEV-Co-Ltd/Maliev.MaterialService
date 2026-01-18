using Maliev.Aspire.ServiceDefaults.IAM;

namespace Maliev.MaterialService.Api.Services.Auth;

/// <summary>
/// Registers Material Service permissions and roles with the centralized IAM service on startup.
/// </summary>
public class MaterialIAMRegistrationService : IAMRegistrationService
{
    private const string ServiceNameValue = "material";

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialIAMRegistrationService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger.</param>
    public MaterialIAMRegistrationService(
        IConfiguration configuration,
        ILogger<MaterialIAMRegistrationService> logger)
        : base(configuration, logger, ServiceNameValue)
    {
    }

    /// <summary>
    /// Gets the list of permissions to register.
    /// </summary>
    /// <returns>A collection of permission registrations.</returns>
    protected override IEnumerable<PermissionRegistration> GetPermissions()
    {
        return MaterialPermissions.AllWithDescriptions.Select(p => new PermissionRegistration
        {
            PermissionId = p.Key,
            Description = p.Value
        });
    }

    /// <summary>
    /// Gets the list of predefined roles to register.
    /// </summary>
    /// <returns>A collection of role registrations.</returns>
    protected override IEnumerable<RoleRegistration> GetPredefinedRoles()
    {
        return MaterialPredefinedRoles.All.Select(r => new RoleRegistration
        {
            RoleId = r.RoleId,
            Description = r.Description,
            PermissionIds = r.Permissions.ToList(),
            IsCustom = false
        });
    }
}
