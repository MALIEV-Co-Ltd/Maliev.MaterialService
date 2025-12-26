using System.Text.Json.Serialization;
using Maliev.MessagingContracts.Generated;

namespace Maliev.MaterialService.Api.Contracts.Messaging;

/// <summary>
/// Payload for MaterialCreatedEvent
/// </summary>
public record MaterialCreatedEventPayload(
    [property: JsonPropertyName("materialId")] Guid MaterialId,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt);

/// <summary>
/// Event published when a new material is created.
/// </summary>
public record MaterialCreatedEvent(
    Guid MessageId,
    string MessageName,
    MessageType MessageType,
    string MessageVersion,
    string PublishedBy,
    IReadOnlyList<string> ConsumedBy,
    Guid CorrelationId,
    Guid? CausationId,
    DateTimeOffset OccurredAtUtc,
    bool IsPublic,
    [property: JsonPropertyName("payload")] MaterialCreatedEventPayload Payload
) : BaseMessage(MessageId, MessageName, MessageType, MessageVersion, PublishedBy, ConsumedBy, CorrelationId, CausationId, OccurredAtUtc, IsPublic);

/// <summary>
/// Payload for MaterialUpdatedEvent
/// </summary>
public record MaterialUpdatedEventPayload(
    [property: JsonPropertyName("materialId")] Guid MaterialId,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("updatedAt")] DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("version")] int Version);

/// <summary>
/// Event published when an existing material is updated.
/// </summary>
public record MaterialUpdatedEvent(
    Guid MessageId,
    string MessageName,
    MessageType MessageType,
    string MessageVersion,
    string PublishedBy,
    IReadOnlyList<string> ConsumedBy,
    Guid CorrelationId,
    Guid? CausationId,
    DateTimeOffset OccurredAtUtc,
    bool IsPublic,
    [property: JsonPropertyName("payload")] MaterialUpdatedEventPayload Payload
) : BaseMessage(MessageId, MessageName, MessageType, MessageVersion, PublishedBy, ConsumedBy, CorrelationId, CausationId, OccurredAtUtc, IsPublic);

/// <summary>
/// Payload for MaterialDeletedEvent
/// </summary>
public record MaterialDeletedEventPayload(
    [property: JsonPropertyName("materialId")] Guid MaterialId,
    [property: JsonPropertyName("deletedAt")] DateTimeOffset DeletedAt);

/// <summary>
/// Event published when a material is deleted (soft-deleted).
/// </summary>
public record MaterialDeletedEvent(
    Guid MessageId,
    string MessageName,
    MessageType MessageType,
    string MessageVersion,
    string PublishedBy,
    IReadOnlyList<string> ConsumedBy,
    Guid CorrelationId,
    Guid? CausationId,
    DateTimeOffset OccurredAtUtc,
    bool IsPublic,
    [property: JsonPropertyName("payload")] MaterialDeletedEventPayload Payload
) : BaseMessage(MessageId, MessageName, MessageType, MessageVersion, PublishedBy, ConsumedBy, CorrelationId, CausationId, OccurredAtUtc, IsPublic);
