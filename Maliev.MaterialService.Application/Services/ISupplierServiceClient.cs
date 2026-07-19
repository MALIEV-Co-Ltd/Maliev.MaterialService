namespace Maliev.MaterialService.Application.Services;

/// <summary>
/// Client for communicating with the external Supplier Service.
/// </summary>
public interface ISupplierServiceClient
{
    /// <summary>
    /// Gets the authoritative Supplier projection required by MaterialService.
    /// </summary>
    /// <param name="supplierId">The supplier ID to validate.</param>
    /// <param name="cancellationToken">Cancellation token for the outbound request.</param>
    /// <returns>The supplier projection, or null when SupplierService returns not found.</returns>
    Task<SupplierReference?> GetSupplierAsync(
        Guid supplierId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Minimal authoritative SupplierService data projected into MaterialService.
/// </summary>
/// <param name="Id">Supplier identifier.</param>
/// <param name="CompanyName">Current supplier company name.</param>
/// <param name="IsActive">Whether the authoritative supplier is active.</param>
public sealed record SupplierReference(Guid Id, string CompanyName, bool IsActive);
