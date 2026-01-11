namespace Maliev.MaterialService.Api.Services.Auth;

/// <summary>
/// Constants for Material Service permissions.
/// Follows GCP-style naming: {service}.{resource}.{action}
/// </summary>
public static class MaterialPermissions
{
    // Material Operations
    /// <summary>Permission to create new materials.</summary>
    public const string MaterialsCreate = "material.materials.create";
    /// <summary>Permission to read material details.</summary>
    public const string MaterialsRead = "material.materials.read";
    /// <summary>Permission to update material information.</summary>
    public const string MaterialsUpdate = "material.materials.update";
    /// <summary>Permission to delete materials.</summary>
    public const string MaterialsDelete = "material.materials.delete";
    /// <summary>Permission to export material data.</summary>
    public const string MaterialsExport = "material.materials.export";

    // Inventory Operations
    /// <summary>Permission to view inventory levels.</summary>
    public const string InventoryView = "material.inventory.view";
    /// <summary>Permission to adjust inventory quantities.</summary>
    public const string InventoryAdjust = "material.inventory.adjust";
    /// <summary>Permission to transfer materials between locations.</summary>
    public const string InventoryTransfer = "material.inventory.transfer";
    /// <summary>Permission to perform inventory counts.</summary>
    public const string InventoryCount = "material.inventory.count";

    // Category Operations
    /// <summary>Permission to create material categories.</summary>
    public const string CategoriesCreate = "material.categories.create";
    /// <summary>Permission to read categories.</summary>
    public const string CategoriesRead = "material.categories.read";
    /// <summary>Permission to update categories.</summary>
    public const string CategoriesUpdate = "material.categories.update";
    /// <summary>Permission to delete categories.</summary>
    public const string CategoriesDelete = "material.categories.delete";

    // Supplier Operations
    /// <summary>Permission to read supplier reference counts.</summary>
    public const string SuppliersRead = "material.suppliers.read";

    /// <summary>
    /// Collection of all defined material permissions with descriptions.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> AllWithDescriptions = new Dictionary<string, string>
    {
        { MaterialsCreate, "Create new materials" },
        { MaterialsRead, "Read material details" },
        { MaterialsUpdate, "Update material information" },
        { MaterialsDelete, "Delete materials" },
        { MaterialsExport, "Export material data" },
        { InventoryView, "View inventory levels" },
        { InventoryAdjust, "Adjust inventory quantities" },
        { InventoryTransfer, "Transfer materials between locations" },
        { InventoryCount, "Perform inventory counts" },
        { CategoriesCreate, "Create material categories" },
        { CategoriesRead, "Read categories" },
        { CategoriesUpdate, "Update categories" },
        { CategoriesDelete, "Delete categories" },
        { SuppliersRead, "Read supplier reference counts" }
    };

    /// <summary>All available permission codes</summary>
    public static IEnumerable<string> All => AllWithDescriptions.Keys;
}
