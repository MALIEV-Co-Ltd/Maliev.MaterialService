using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Maliev.MaterialService.Api.Middleware;

/// <summary>
/// Middleware to add security headers to responses
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of SecurityHeadersMiddleware
    /// </summary>
    /// <param name="next">Next middleware in the pipeline</param>
    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    /// <param name="context">HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Allow relaxed CSP for Scalar documentation endpoints
        if (context.Request.Path.StartsWithSegments("/materials/scalar") ||
            context.Request.Path.StartsWithSegments("/materials/openapi"))
        {
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                "font-src 'self' https://fonts.gstatic.com; " +
                "img-src 'self' data: https:; " +
                "connect-src 'self';");
        }
        else
        {
            // Strict CSP for all other endpoints
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
        }

        await _next(context);
    }
}
