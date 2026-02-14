using System;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testcontainers.RabbitMq;
using Xunit;

namespace Maliev.MaterialService.Tests;

public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly RedisContainer _redisContainer;
    private readonly RabbitMqContainer _rabbitMqContainer;

    public string PostgresConnectionString => _postgresContainer.GetConnectionString();
    public string RedisConnectionString => _redisContainer.GetConnectionString();
    public string RabbitMqConnectionString =>
        $"amqp://guest:guest@{_rabbitMqContainer.Hostname}:{_rabbitMqContainer.GetMappedPublicPort(5672)}";

    public string RabbitMqHost => _rabbitMqContainer.Hostname;
    public int RabbitMqPort => _rabbitMqContainer.GetMappedPublicPort(5672);

    public IntegrationTestFixture()
    {
        _postgresContainer = new PostgreSqlBuilder().WithName("postgres:18-alpine")
            .WithDatabase("material_test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

        _redisContainer = new RedisBuilder().WithName("redis:8.4-alpine")
            .Build();

        _rabbitMqContainer = new RabbitMqBuilder().WithName("rabbitmq:4.2-alpine")
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();
    }

    public async Task InitializeAsync()
    {
        // Start all containers in parallel
        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _redisContainer.StartAsync(),
            _rabbitMqContainer.StartAsync()
        );
    }

    public async Task DisposeAsync()
    {
        // Stop and dispose all containers
        await Task.WhenAll(
            _postgresContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask(),
            _rabbitMqContainer.DisposeAsync().AsTask()
        );
    }
}
