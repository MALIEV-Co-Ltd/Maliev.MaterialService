using System.Net;
using Maliev.MaterialService.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Maliev.MaterialService.Tests.Unit.Services;

/// <summary>
/// Verifies the protected SupplierService lookup contract.
/// </summary>
public sealed class SupplierServiceClientTests
{
    /// <summary>
    /// A successful supplier lookup uses the versioned protected route.
    /// </summary>
    [Fact]
    public async Task GetSupplierAsync_Success_UsesVersionedValidationRoute()
    {
        var supplierId = Guid.NewGuid();
        var handler = new CapturingHandler(
            HttpStatusCode.OK,
            $$"""{"id":"{{supplierId}}","companyName":"Supplier One","isActive":true}""");
        var client = CreateClient(handler);

        var supplier = await client.GetSupplierAsync(supplierId, CancellationToken.None);

        Assert.NotNull(supplier);
        Assert.Equal(supplierId, supplier.Id);
        Assert.Equal("Supplier One", supplier.CompanyName);
        Assert.Equal($"/supplier/v1/suppliers/{supplierId}/validate", handler.RequestUri?.AbsolutePath);
    }

    /// <summary>
    /// Only an authoritative 404 is translated into a missing supplier result.
    /// </summary>
    [Fact]
    public async Task GetSupplierAsync_NotFound_ReturnsNull()
    {
        var client = CreateClient(new CapturingHandler(HttpStatusCode.NotFound));

        var supplier = await client.GetSupplierAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(supplier);
    }

    /// <summary>
    /// Authorization and dependency failures must not masquerade as missing suppliers.
    /// </summary>
    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task GetSupplierAsync_DependencyFailure_Throws(HttpStatusCode statusCode)
    {
        var client = CreateClient(new CapturingHandler(statusCode));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.GetSupplierAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal(statusCode, exception.StatusCode);
    }

    /// <summary>
    /// Caller cancellation must reach the outbound Supplier request.
    /// </summary>
    [Fact]
    public async Task GetSupplierAsync_Cancelled_PropagatesCancellation()
    {
        var handler = new CancellationAwareHandler();
        var client = CreateClient(handler);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetSupplierAsync(Guid.NewGuid(), cancellation.Token));

        Assert.True(handler.ObservedCancellation);
    }

    private static SupplierServiceClient CreateClient(HttpMessageHandler handler) =>
        new(
            new HttpClient(handler)
            {
                BaseAddress = new Uri("https://supplier.test")
            },
            NullLogger<SupplierServiceClient>.Instance);

    private sealed class CapturingHandler(HttpStatusCode statusCode, string? body = null) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = body is null ? null : new StringContent(body)
            });
        }
    }

    private sealed class CancellationAwareHandler : HttpMessageHandler
    {
        public bool ObservedCancellation { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            ObservedCancellation = cancellationToken.IsCancellationRequested;
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
