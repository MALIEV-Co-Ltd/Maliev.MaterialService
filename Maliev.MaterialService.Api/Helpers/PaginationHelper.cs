using Maliev.MaterialService.Api.Models;
using Microsoft.AspNetCore.Http;

namespace Maliev.MaterialService.Api.Helpers;

/// <summary>
/// Helper class for pagination operations.
/// </summary>
public static class PaginationHelper
{
    /// <summary>
    /// Adds pagination headers to the HTTP response.
    /// </summary>
    /// <param name="response">The HTTP response to add headers to.</param>
    /// <param name="pagedResult">The paged result containing pagination information.</param>
    public static void AddPaginationHeaders(HttpResponse response, PagedResult<object> pagedResult)
    {
        response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(new
        {
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage
        });
    }

    /// <summary>
    /// Adds pagination headers to the HTTP response.
    /// </summary>
    /// <typeparam name="T">The type of items in the paged result.</typeparam>
    /// <param name="response">The HTTP response to add headers to.</param>
    /// <param name="pagedResult">The paged result containing pagination information.</param>
    public static void AddPaginationHeaders<T>(HttpResponse response, PagedResult<T> pagedResult)
    {
        response.Headers["X-Pagination"] = System.Text.Json.JsonSerializer.Serialize(new
        {
            pagedResult.PageNumber,
            pagedResult.PageSize,
            pagedResult.TotalItems,
            pagedResult.TotalPages,
            pagedResult.HasPreviousPage,
            pagedResult.HasNextPage
        });
    }
}