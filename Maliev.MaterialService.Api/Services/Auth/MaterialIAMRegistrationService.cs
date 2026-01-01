using Maliev.Aspire.ServiceDefaults.IAM;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Api.Services.Auth;

/// <summary>
/// Registers Material Service permissions and roles with the centralized IAM service on startup.
/// </summary>
public class MaterialIAMRegistrationService : IAMRegistrationService
{
    private const string ServiceName = "MaterialService";

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialIAMRegistrationService"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="logger">The logger.</param>
    public MaterialIAMRegistrationService(
        IHttpClientFactory httpClientFactory,
        ILogger<MaterialIAMRegistrationService> logger)
        : base(httpClientFactory, logger, ServiceName)
    {
    }

    /// <summary>
    /// Gets the list of permissions to register.
    /// </summary>
    /// <returns>A collection of permission registrations.</returns>
    protected override IEnumerable<PermissionRegistration> GetPermissions()
    {
        return new List<PermissionRegistration>
        {
            // Material Operations
            new() { PermissionId = "material.materials.create", Description = "Create new materials" },
            new() { PermissionId = "material.materials.read", Description = "Read material details" },
            new() { PermissionId = "material.materials.update", Description = "Update material information" },
            new() { PermissionId = "material.materials.delete", Description = "Delete materials" },
            new() { PermissionId = "material.materials.export", Description = "Export material data" },

            // Inventory Operations
            new() { PermissionId = "material.inventory.view", Description = "View inventory levels" },
            new() { PermissionId = "material.inventory.adjust", Description = "Adjust inventory quantities" },
            new() { PermissionId = "material.inventory.transfer", Description = "Transfer materials between locations" },
            new() { PermissionId = "material.inventory.count", Description = "Perform inventory counts" },

            // Category Operations
            new() { PermissionId = "material.categories.create", Description = "Create material categories" },
            new() { PermissionId = "material.categories.read", Description = "Read categories" },
            new() { PermissionId = "material.categories.update", Description = "Update categories" },
            new() { PermissionId = "material.categories.delete", Description = "Delete categories" },

            // Supplier Operations
            new() { PermissionId = "material.suppliers.read", Description = "Read supplier reference counts" }
        };
    }

    /// <summary>
    /// Gets the list of predefined roles to register.
    /// </summary>
    /// <returns>A collection of role registrations.</returns>
    protected override IEnumerable<RoleRegistration> GetPredefinedRoles()
    {
        var allPermissions = GetPermissions().Select(p => p.PermissionId).ToList();

        return new List<RoleRegistration>
        {
            new()
            {
                RoleId = "roles.material.admin",
                Description = "Full access to all material service operations",
                PermissionIds = allPermissions
            },
            new()
            {
                RoleId = "roles.material.manager",
                Description = "Manage materials, inventory, and categories",
                PermissionIds = new List<string>
                {
                    "material.materials.create",
                    "material.materials.read",
                    "material.materials.update",
                    "material.materials.export",
                    "material.inventory.view",
                    "material.inventory.adjust",
                    "material.inventory.transfer",
                    "material.inventory.count",
                    "material.categories.create",
                    "material.categories.read",
                    "material.categories.update",
                    "material.categories.delete",
                    "material.suppliers.read"
                }
            },
            new()
            {
                RoleId = "roles.material.clerk",
                Description = "Daily inventory operations and material recording",
                PermissionIds = new List<string>
                {
                    "material.materials.create",
                    "material.materials.read",
                    "material.materials.update",
                    "material.inventory.view",
                    "material.inventory.count"
                }
            },
            new()
            {
                RoleId = "roles.material.viewer",
                Description = "Read-only access to materials and inventory",
                PermissionIds = new List<string>
                {
                    "material.materials.read",
                    "material.inventory.view",
                    "material.categories.read",
                    "material.suppliers.read"
                }
            }
        };
    }
}
