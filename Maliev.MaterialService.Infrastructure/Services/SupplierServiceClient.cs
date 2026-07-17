using System.Net;
using Maliev.MaterialService.Application.Services;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Infrastructure.Services;

/// <summary>
/// HTTP client for validating suppliers against the external Supplier Service.
/// </summary>
public class SupplierServiceClient : ISupplierServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SupplierServiceClient> _logger;

    /// <summary>
    /// Initializes a new instance of the Supplier Service client.
    /// </summary>
    /// <param name="httpClient">HTTP client instance.</param>
    /// <param name="logger">Logger instance.</param>
    public SupplierServiceClient(HttpClient httpClient, ILogger<SupplierServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateSupplierExistsAsync(
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"/supplier/v1/suppliers/{supplierId}",
            cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        _logger.LogError(
            "Supplier Service lookup failed for {SupplierId} with status {StatusCode}",
            supplierId,
            response.StatusCode);
        response.EnsureSuccessStatusCode();
        throw new InvalidOperationException("Unreachable after EnsureSuccessStatusCode.");
    }
}
