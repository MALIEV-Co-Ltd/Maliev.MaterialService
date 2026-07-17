using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
    public async Task<SupplierReference?> GetSupplierAsync(
        Guid supplierId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"/supplier/v1/suppliers/{supplierId}/reference",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        SupplierValidationResponse? supplier;
        try
        {
            supplier = await response.Content.ReadFromJsonAsync<SupplierValidationResponse>(
                cancellationToken);
        }
        catch (Exception exception) when (exception is JsonException or NotSupportedException)
        {
            _logger.LogError(
                exception,
                "Supplier Service returned a malformed projection for {SupplierId}",
                supplierId);
            throw new HttpRequestException(
                "Supplier Service returned an invalid supplier projection.",
                exception,
                HttpStatusCode.BadGateway);
        }
        if (supplier is null ||
            supplier.Id != supplierId ||
            string.IsNullOrWhiteSpace(supplier.CompanyName) ||
            !supplier.IsActive)
        {
            _logger.LogError(
                "Supplier Service returned an invalid projection for {SupplierId}",
                supplierId);
            throw new HttpRequestException(
                "Supplier Service returned an invalid supplier projection.",
                inner: null,
                HttpStatusCode.BadGateway);
        }

        return new SupplierReference(supplier.Id, supplier.CompanyName);
    }

    private sealed record SupplierValidationResponse(
        Guid Id,
        string CompanyName,
        bool IsActive);
}
