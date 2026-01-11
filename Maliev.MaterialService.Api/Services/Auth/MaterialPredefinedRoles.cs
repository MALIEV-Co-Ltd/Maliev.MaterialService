namespace Maliev.MaterialService.Api.Services.Auth;

/// <summary>
/// Predefined roles for the Material Service.
/// </summary>
public static class MaterialPredefinedRoles
{
    /// <summary>Role for administrators with full access.</summary>
    public const string Admin = "roles.material.admin";
    /// <summary>Role for material managers.</summary>
    public const string Manager = "roles.material.manager";
    /// <summary>Role for inventory clerks.</summary>
    public const string Clerk = "roles.material.clerk";
    /// <summary>Role for read-only viewers.</summary>
    public const string Viewer = "roles.material.viewer";

    /// <summary>
    /// Collection of all predefined roles for the Material Service.
    /// </summary>
    public static readonly IReadOnlyList<(string RoleId, string Description, string[] Permissions)> All = new List<(string, string, string[])>
    {
        (Admin, "Full access to all material service operations", MaterialPermissions.All.ToArray()),

        (Manager, "Manage materials, inventory, and categories", new[]
        {
            MaterialPermissions.MaterialsCreate,
            MaterialPermissions.MaterialsRead,
            MaterialPermissions.MaterialsUpdate,
            MaterialPermissions.MaterialsExport,
            MaterialPermissions.InventoryView,
            MaterialPermissions.InventoryAdjust,
            MaterialPermissions.InventoryTransfer,
            MaterialPermissions.InventoryCount,
            MaterialPermissions.CategoriesCreate,
            MaterialPermissions.CategoriesRead,
            MaterialPermissions.CategoriesUpdate,
            MaterialPermissions.CategoriesDelete,
            MaterialPermissions.SuppliersRead
        }),

        (Clerk, "Daily inventory operations and material recording", new[]
        {
            MaterialPermissions.MaterialsCreate,
            MaterialPermissions.MaterialsRead,
            MaterialPermissions.MaterialsUpdate,
            MaterialPermissions.InventoryView,
            MaterialPermissions.InventoryCount
        }),

        (Viewer, "Read-only access to materials and inventory", new[]
        {
            MaterialPermissions.MaterialsRead,
            MaterialPermissions.InventoryView,
            MaterialPermissions.CategoriesRead,
            MaterialPermissions.SuppliersRead
        })
    };
}
