using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Maliev.MaterialService.Api.HealthChecks;

/// <summary>
/// Health check for external HTTP services.
/// </summary>
public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceName;
    private readonly string _serviceUrl;
    private readonly ILogger<ExternalServiceHealthCheck> _logger;

    /// <summary>
    /// Initializes a new instance of the ExternalServiceHealthCheck class.
    /// </summary>
    /// <param name="httpClient">The HTTP client to use for checking the service.</param>
    /// <param name="serviceName">The name of the service being checked.</param>
    /// <param name="serviceUrl">The URL of the service being checked.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public ExternalServiceHealthCheck(
        HttpClient httpClient,
        string serviceName,
        string serviceUrl,
        ILogger<ExternalServiceHealthCheck> logger)
    {
        _httpClient = httpClient;
        _serviceName = serviceName;
        _serviceUrl = serviceUrl;
        _logger = logger;
    }

    /// <summary>
    /// Checks the health of the external service.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The health check result.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking health of external service {ServiceName} at {ServiceUrl}", _serviceName, _serviceUrl);

            // Send a GET request to the service
            var response = await _httpClient.GetAsync(_serviceUrl, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("External service {ServiceName} is healthy", _serviceName);
                return HealthCheckResult.Healthy($"{_serviceName} is accessible", new Dictionary<string, object>
                {
                    { "service", _serviceName },
                    { "url", _serviceUrl },
                    { "statusCode", (int)response.StatusCode }
                });
            }
            else
            {
                _logger.LogWarning("External service {ServiceName} returned status code {StatusCode}", _serviceName, response.StatusCode);
                return HealthCheckResult.Degraded($"{_serviceName} returned status code {(int)response.StatusCode}", null, new Dictionary<string, object>
                {
                    { "service", _serviceName },
                    { "url", _serviceUrl },
                    { "statusCode", (int)response.StatusCode }
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External service {ServiceName} health check failed", _serviceName);
            return HealthCheckResult.Unhealthy($"{_serviceName} is not accessible", ex, new Dictionary<string, object>
            {
                { "service", _serviceName },
                { "url", _serviceUrl }
            });
        }
    }
}