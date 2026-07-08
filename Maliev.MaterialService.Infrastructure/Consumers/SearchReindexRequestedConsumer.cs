using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Infrastructure.Search;
using Maliev.MessagingContracts.Contracts.Search;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Infrastructure.Consumers;

/// <summary>
/// Republishes material search documents when SearchService requests a reindex.
/// </summary>
public class SearchReindexRequestedConsumer : IConsumer<SearchReindexRequestedCommand>
{
    private const string SourceService = "MaterialService";
    private readonly MaterialDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<SearchReindexRequestedConsumer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchReindexRequestedConsumer"/> class.
    /// </summary>
    /// <param name="context">Material database context.</param>
    /// <param name="publishEndpoint">MassTransit publish endpoint.</param>
    /// <param name="logger">Logger instance.</param>
    public SearchReindexRequestedConsumer(
        MaterialDbContext context,
        IPublishEndpoint publishEndpoint,
        ILogger<SearchReindexRequestedConsumer> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task Consume(ConsumeContext<SearchReindexRequestedCommand> context)
    {
        if (!ShouldHandle(context.Message.Payload.SourceService))
        {
            return;
        }

        var count = 0;
        var occurredAtUtc = DateTimeOffset.UtcNow;

        var materials = await _context.Materials
            .AsNoTracking()
            .Include(item => item.Supplier)
            .Include(item => item.ManufacturingProcesses)
            .Include(item => item.AvailableColors)
            .Include(item => item.PostProcessingMethods)
            .Where(item => item.Active)
            .AsSplitQuery()
            .ToListAsync(context.CancellationToken);

        var documents = materials
            .Select(material => MaterialSearchDocumentMapper.ToUpsertEvent(material, occurredAtUtc))
            .ToList();

        foreach (var document in documents)
        {
            await _publishEndpoint.Publish(
                document,
                context.CancellationToken);
            count++;
        }

        _logger.LogInformation("Republished {Count} material search documents", count);
    }

    private static bool ShouldHandle(string? sourceService)
    {
        return string.IsNullOrWhiteSpace(sourceService) ||
            string.Equals(sourceService, SourceService, StringComparison.OrdinalIgnoreCase);
    }
}
