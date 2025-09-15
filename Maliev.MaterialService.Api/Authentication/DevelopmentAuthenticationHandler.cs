using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Maliev.MaterialService.Api.Authentication;

/// <summary>
/// Development authentication handler that allows all requests.
/// This handler is used in development environments to bypass authentication.
/// </summary>
public class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevelopmentAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">The options monitor.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    public DevelopmentAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Handles the authentication process by creating a fake identity for development.
    /// </summary>
    /// <returns>An authentication result with a fake identity.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Create a fake identity for development
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "DevelopmentUser"),
            new Claim(ClaimTypes.NameIdentifier, "dev-user-id"),
            new Claim("role", "Developer")
        };

        var identity = new ClaimsIdentity(claims, "Development");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Development");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}