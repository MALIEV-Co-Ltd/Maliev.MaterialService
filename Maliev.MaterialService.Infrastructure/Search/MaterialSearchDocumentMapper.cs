using System.Globalization;
using Maliev.MaterialService.Application.Authorization;
using Maliev.MaterialService.Domain.Entities;
using Maliev.MessagingContracts.Contracts.Search;
using Maliev.MessagingContracts.Contracts.Shared;

namespace Maliev.MaterialService.Infrastructure.Search;

/// <summary>
/// Maps material catalog records to centralized global search documents.
/// </summary>
public static class MaterialSearchDocumentMapper
{
    private const string SourceService = "MaterialService";
    private const string ResourceType = "material";

    /// <summary>
    /// Creates a search upsert event for a material.
    /// </summary>
    /// <param name="material">Material to index.</param>
    /// <param name="occurredAtUtc">Timestamp for the source change.</param>
    /// <returns>A centralized search upsert event.</returns>
    public static SearchDocumentUpsertedEvent ToUpsertEvent(Material material, DateTimeOffset occurredAtUtc)
    {
        var subtitleParts = CompactKeywords(material.Code, material.Category, material.Supplier?.Name);
        var summary = string.Join(" ",
            material.Description,
            material.PricePerUnit.ToString("N2", CultureInfo.InvariantCulture),
            material.StockLevel.ToString(CultureInfo.InvariantCulture),
            string.Join(" ", material.ManufacturingProcesses.Select(process => process.Name)),
            string.Join(" ", material.AvailableColors.Select(color => color.Name)),
            string.Join(" ", material.PostProcessingMethods.Select(method => method.Name)))
            .Trim();

        var keywords = CompactKeywords(
            material.Id.ToString(),
            material.Code,
            material.Name,
            material.Category,
            material.SupplierId?.ToString(),
            material.Supplier?.Name,
            material.StockLevel.ToString(CultureInfo.InvariantCulture),
            material.Active ? "Active" : "Inactive");

        return new SearchDocumentUpsertedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(SearchDocumentUpsertedEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: SourceService,
            ConsumedBy: ["SearchService"],
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: occurredAtUtc,
            IsPublic: false,
            Payload: new SearchDocumentUpsertedEventPayload(
                SourceService: SourceService,
                ResourceType: ResourceType,
                ResourceId: material.Id.ToString(),
                Title: material.Name,
                Subtitle: subtitleParts.Count == 0 ? null : string.Join(" - ", subtitleParts),
                Summary: string.IsNullOrWhiteSpace(summary) ? null : summary,
                Keywords: keywords,
                Status: material.Active ? "Active" : "Inactive",
                RequiredPermission: MaterialPermissions.MaterialsRead,
                OccurredAtUtc: occurredAtUtc));
    }

    /// <summary>
    /// Creates a search delete event for a material.
    /// </summary>
    /// <param name="materialId">Material identifier.</param>
    /// <param name="occurredAtUtc">Timestamp for the source change.</param>
    /// <returns>A centralized search delete event.</returns>
    public static SearchDocumentDeletedEvent ToDeletedEvent(Guid materialId, DateTimeOffset occurredAtUtc)
    {
        return new SearchDocumentDeletedEvent(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(SearchDocumentDeletedEvent),
            MessageType: MessageType.Event,
            MessageVersion: "1.0.0",
            PublishedBy: SourceService,
            ConsumedBy: ["SearchService"],
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: occurredAtUtc,
            IsPublic: false,
            Payload: new SearchDocumentDeletedEventPayload(
                SourceService: SourceService,
                ResourceType: ResourceType,
                ResourceId: materialId.ToString(),
                OccurredAtUtc: occurredAtUtc));
    }

    private static IReadOnlyList<string> CompactKeywords(params string?[] values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
