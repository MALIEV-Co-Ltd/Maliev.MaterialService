using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Maliev.Aspire.ServiceDefaults;
using Maliev.Aspire.ServiceDefaults.Authorization;
using Maliev.Aspire.ServiceDefaults.IAM;
using Maliev.MaterialService.Application.Services;
using Maliev.MaterialService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Maliev.MaterialService.Tests.Unit;

/// <summary>
/// Verifies MaterialService's centrally issued workload-authentication boundary.
/// </summary>
public sealed class ServiceAuthenticationWiringTests
{
    private const string ExpectedToken = "centrally-issued-material-token";

    /// <summary>
    /// Startup must opt into AuthService exchange and remove the legacy local signer.
    /// </summary>
    [Fact]
    public void Program_RegistersMaterialExchangeWithoutLegacySigner()
    {
        var source = ReadRepositoryFile("Maliev.MaterialService.Api", "Program.cs");

        Assert.Contains("builder.AddAuthServiceTokenExchange(\"MaterialService\");", source, StringComparison.Ordinal);
        Assert.Contains("builder.AddAuthServiceIAMClient();", source, StringComparison.Ordinal);
        Assert.Contains(".AddAuthServiceAuthentication()", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddIAMServiceClient", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddAuthenticatedServiceClient", source, StringComparison.Ordinal);
    }

    /// <summary>
    /// The protected Supplier client must use the AuthService-issued bearer token.
    /// </summary>
    [Fact]
    public async Task SupplierClient_UsesAuthServiceBearer()
    {
        var builder = CreateConfiguredBuilder();
        var filter = new TrackingPrimaryHandlerFilter();
        builder.Services.AddSingleton<IHttpMessageHandlerBuilderFilter>(filter);

        builder.AddAuthServiceTokenExchange("MaterialService");
        builder.Services.AddSingleton<IAuthServiceTokenProvider>(new StubTokenProvider());
        builder.Services.AddHttpClient<ISupplierServiceClient, SupplierServiceClient>("SupplierService", client =>
            {
                client.BaseAddress = new Uri("https://supplier.test");
            })
            .AddAuthServiceAuthentication();

        await using var provider = builder.Services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        using var client = factory.CreateClient("SupplierService");
        using var response = await client.GetAsync("/probe", CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            new AuthenticationHeaderValue("Bearer", ExpectedToken),
            filter.GetCapture("SupplierService").Authorization);
        Assert.True(filter.HasAuthServiceHandler("SupplierService"));
    }

    /// <summary>
    /// Material uses its exact process identity and does not resolve legacy signing services.
    /// </summary>
    [Fact]
    public void MaterialExchange_RegistersExactIdentityWithoutLegacySigningServices()
    {
        var builder = CreateConfiguredBuilder();
        builder.AddAuthServiceTokenExchange("MaterialService");
        builder.AddAuthServiceIAMClient();

        using var provider = builder.Services.BuildServiceProvider();

        Assert.Equal("MaterialService", provider.GetRequiredService<ServiceProcessIdentity>().ServiceName);
        Assert.Null(provider.GetService<IServiceAccountTokenProvider>());
        Assert.Null(provider.GetService<ServiceAccountAuthenticationHandler>());
    }

    /// <summary>
    /// Invalid workload credentials must stop the host rather than permit anonymous fallback.
    /// </summary>
    [Theory]
    [InlineData(null, null)]
    [InlineData("service-material-service", "short")]
    public async Task AuthServiceExchange_InvalidCredentials_FailsClosedAtHostStartup(
        string? clientId,
        string? clientSecret)
    {
        var builder = CreateConfiguredBuilder(clientId, clientSecret);
        builder.AddAuthServiceTokenExchange("MaterialService");

        using var host = builder.Build();

        await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
    }

    /// <summary>
    /// CI must restore the ServiceDefaults release that owns central exchange behavior.
    /// </summary>
    [Fact]
    public void ServiceDefaultsDependency_PinsPublishedCentralExchangeVersion()
    {
        var source = ReadRepositoryFile("Directory.Build.props");

        Assert.Contains(
            "<ServiceDefaultsVersion Condition=\"'$(ServiceDefaultsVersion)' == ''\">1.0.89-alpha</ServiceDefaultsVersion>",
            source,
            StringComparison.Ordinal);

        foreach (var project in new[]
                 {
                     "Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj",
                     "Maliev.MaterialService.Application/Maliev.MaterialService.Application.csproj",
                     "Maliev.MaterialService.Infrastructure/Maliev.MaterialService.Infrastructure.csproj",
                     "Maliev.MaterialService.Tests/Maliev.MaterialService.Tests.csproj"
                 })
        {
            var projectSource = ReadRepositoryFile(project.Split('/'));
            Assert.Contains(
                "<PackageReference Include=\"Maliev.Aspire.ServiceDefaults\" Version=\"$(ServiceDefaultsVersion)\" />",
                projectSource,
                StringComparison.Ordinal);
        }
    }

    private static HostApplicationBuilder CreateConfiguredBuilder(
        string? clientId = "service-material-service",
        string? clientSecret = "material-test-secret-with-at-least-32-bytes")
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing"
        });

        using var rsa = RSA.Create(2048);
        builder.Configuration["ServiceAuthentication:ClientId"] = clientId;
        builder.Configuration["ServiceAuthentication:ClientSecret"] = clientSecret;
        builder.Configuration["Services:AuthService:BaseUrl"] = "https://auth.test";
        builder.Configuration["Services:IAMService:BaseUrl"] = "https://iam.test";
        builder.Configuration["Jwt:PublicKey"] = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(rsa.ExportSubjectPublicKeyInfoPem()));
        builder.Configuration["Jwt:Issuer"] = "https://api.maliev.com";
        builder.Configuration["Jwt:Audience"] = "https://api.maliev.com";

        return builder;
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            Path.Combine(segments)));

        Assert.True(File.Exists(path), $"Could not find source file: {path}");
        return File.ReadAllText(path);
    }

    private sealed class StubTokenProvider : IAuthServiceTokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(ExpectedToken);
    }

    private sealed class AuthorizationCaptureHandler : HttpMessageHandler
    {
        public AuthenticationHeaderValue? Authorization { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Authorization = request.Headers.Authorization;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private sealed class TrackingPrimaryHandlerFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly Dictionary<string, AuthorizationCaptureHandler> _captures = new(StringComparer.Ordinal);
        private readonly Dictionary<string, bool> _authHandlers = new(StringComparer.Ordinal);

        public AuthorizationCaptureHandler GetCapture(string clientName) => _captures[clientName];

        public bool HasAuthServiceHandler(string clientName) => _authHandlers[clientName];

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) => builder =>
        {
            next(builder);
            var clientName = builder.Name
                ?? throw new InvalidOperationException("Every HttpClientFactory handler must have a client name.");
            _authHandlers[clientName] = builder.AdditionalHandlers.Any(
                handler => handler is AuthServiceTokenExchangeHandler);

            for (var index = builder.AdditionalHandlers.Count - 1; index >= 0; index--)
            {
                if (builder.AdditionalHandlers[index].GetType().FullName?.Contains(
                        "ServiceDiscovery",
                        StringComparison.Ordinal) == true ||
                    builder.AdditionalHandlers[index].GetType().FullName?.Contains(
                        "ResolvingHttpDelegatingHandler",
                        StringComparison.Ordinal) == true)
                {
                    builder.AdditionalHandlers.RemoveAt(index);
                }
            }

            var capture = new AuthorizationCaptureHandler();
            _captures[clientName] = capture;
            builder.PrimaryHandler = capture;
        };
    }
}
