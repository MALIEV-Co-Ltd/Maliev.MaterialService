using System.Data.Common;
using Maliev.MaterialService.Infrastructure.Consumers;
using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Tests.Fixtures;
using Maliev.MaterialService.Tests.Helpers;
using Maliev.MessagingContracts.Contracts.Search;
using Maliev.MessagingContracts.Contracts.Shared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Maliev.MaterialService.Tests.Integration;

[Collection("Sequential")]
public class SearchReindexRequestedConsumerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    public SearchReindexRequestedConsumerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Consume_ReindexRequest_CompletesDatabaseReadBeforePublishing()
    {
        await _factory.CleanDatabaseAsync();
        await using var baseContext = _factory.CreateDbContext();
        var connectionString = baseContext.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Database connection string not found.");

        var activeReaders = new ActiveReaderInterceptor();
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        optionsBuilder
            .UseNpgsql(connectionString)
            .AddInterceptors(activeReaders);

        await using var dbContext = new MaterialDbContext(optionsBuilder.Options, metricsInterceptor: null);
        SeedData.Initialize(dbContext);
        activeReaders.Reset();

        var publishEndpoint = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        var publishCount = 0;

        publishEndpoint
            .Setup(endpoint => endpoint.Publish(
                It.IsAny<SearchDocumentUpsertedEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns<SearchDocumentUpsertedEvent, CancellationToken>(async (_, cancellationToken) =>
            {
                if (Interlocked.Increment(ref publishCount) == 1)
                {
                    Assert.Equal(0, activeReaders.ActiveCount);
                }

                await Task.CompletedTask;
            });

        var consumeContext = new Mock<ConsumeContext<SearchReindexRequestedCommand>>();
        consumeContext
            .SetupGet(context => context.Message)
            .Returns(CreateReindexRequest());
        consumeContext
            .SetupGet(context => context.CancellationToken)
            .Returns(CancellationToken.None);

        var consumer = new SearchReindexRequestedConsumer(
            dbContext,
            publishEndpoint.Object,
            NullLogger<SearchReindexRequestedConsumer>.Instance);

        await consumer.Consume(consumeContext.Object);

        Assert.Equal(3, publishCount);
    }

    private sealed class ActiveReaderInterceptor : DbCommandInterceptor
    {
        private int _activeCount;

        public int ActiveCount => Volatile.Read(ref _activeCount);

        public void Reset()
        {
            Volatile.Write(ref _activeCount, 0);
        }

        public override DbDataReader ReaderExecuted(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result)
        {
            Interlocked.Increment(ref _activeCount);
            return base.ReaderExecuted(command, eventData, result);
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _activeCount);
            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }

        public override InterceptionResult DataReaderDisposing(
            DbCommand command,
            DataReaderDisposingEventData eventData,
            InterceptionResult result)
        {
            Interlocked.Decrement(ref _activeCount);
            return base.DataReaderDisposing(command, eventData, result);
        }
    }

    private static SearchReindexRequestedCommand CreateReindexRequest()
    {
        var occurredAtUtc = DateTimeOffset.UtcNow;

        return new SearchReindexRequestedCommand(
            MessageId: Guid.NewGuid(),
            MessageName: nameof(SearchReindexRequestedCommand),
            MessageType: MessageType.Command,
            MessageVersion: "1.0.0",
            PublishedBy: "SearchService",
            ConsumedBy: ["MaterialService"],
            CorrelationId: Guid.NewGuid(),
            CausationId: null,
            OccurredAtUtc: occurredAtUtc,
            IsPublic: false,
            Payload: new SearchReindexRequestedCommandPayload(
                SourceService: "MaterialService",
                RequestedBy: "SearchService",
                RequestedAtUtc: occurredAtUtc));
    }
}
