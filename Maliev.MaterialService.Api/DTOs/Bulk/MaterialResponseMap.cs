using CsvHelper.Configuration;
using Maliev.MaterialService.Api.DTOs.Materials;

namespace Maliev.MaterialService.Api.DTOs.Bulk;

/// <summary>
/// CSV mapping for MaterialResponse to control exported fields and their order.
/// </summary>
public sealed class MaterialResponseMap : ClassMap<MaterialResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialResponseMap"/> class.
    /// </summary>
    public MaterialResponseMap()
    {
        Map(m => m.Code);
        Map(m => m.Name);
        Map(m => m.PricePerUnit);
        Map(m => m.StockLevel);
    }
}