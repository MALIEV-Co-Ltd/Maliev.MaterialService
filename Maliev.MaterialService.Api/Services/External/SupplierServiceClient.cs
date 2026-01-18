using System.Net;

namespace Maliev.MaterialService.Api.Services.External;

/// <summary>
/// HTTP client for validating suppliers against the external Supplier Service
/// </summary>
public class SupplierServiceClient : ISupplierServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SupplierServiceClient> _logger;

    /// <summary>
    /// Initializes a new instance of the Supplier Service client
    /// </summary>
    /// <param name="httpClient">HTTP client instance</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="logger">Logger instance</param>
    public SupplierServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<SupplierServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.Timeout = TimeSpan.FromSeconds(60);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateSupplierExistsAsync(Guid supplierId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"suppliers/{supplierId}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            _logger.LogError("Error calling Supplier Service. StatusCode: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while calling Supplier Service for ID {SupplierId}", supplierId);
            return false;
        }
    }
}
