using System;
using System.Threading.Tasks;

namespace Maliev.MaterialService.Api.Services.External;

/// <summary>
/// Client for communicating with the external Supplier Service
/// </summary>
public interface ISupplierServiceClient
{
    /// <summary>
    /// Validates whether a supplier exists in the Supplier Service
    /// </summary>
    /// <param name="supplierId">The supplier ID to validate</param>
    /// <returns>True if the supplier exists, false otherwise</returns>
    Task<bool> ValidateSupplierExistsAsync(Guid supplierId);
}
