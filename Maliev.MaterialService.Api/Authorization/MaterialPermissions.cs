namespace Maliev.MaterialService.Api.Authorization;

/// <summary>
/// Defines permission constants for the Material Service.
/// Follows the {service}.{resource}.{action} naming convention.
/// </summary>
public static class MaterialPermissions
{
    /// <summary>Permission to read materials.</summary>
    public const string MaterialsRead = "material.materials.read";
    /// <summary>Permission to create materials.</summary>
    public const string MaterialsCreate = "material.materials.create";
    /// <summary>Permission to update materials.</summary>
    public const string MaterialsUpdate = "material.materials.update";
    /// <summary>Permission to delete materials.</summary>
    public const string MaterialsDelete = "material.materials.delete";
    /// <summary>Permission to export materials.</summary>
    public const string MaterialsExport = "material.materials.export";

    /// <summary>Permission to read categories.</summary>
    public const string CategoriesRead = "material.categories.read";
    /// <summary>Permission to read suppliers.</summary>
    public const string SuppliersRead = "material.suppliers.read";

    /// <summary>Permission to view inventory.</summary>
    public const string InventoryView = "material.inventory.view";
    /// <summary>Permission to count inventory.</summary>
    public const string InventoryCount = "material.inventory.count";
    /// <summary>Permission to adjust inventory.</summary>
    public const string InventoryAdjust = "material.inventory.adjust";
}
