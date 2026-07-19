using Microsoft.EntityFrameworkCore.Diagnostics;
using Prometheus;
using System.Data.Common;

namespace Maliev.MaterialService.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that records database query metrics to Prometheus.
/// </summary>
public class DatabaseMetricsInterceptor : DbCommandInterceptor
{
    private static readonly Histogram QueryDuration = Metrics.CreateHistogram(
        "db_query_duration_seconds",
        "Duration of database queries in seconds",
        new HistogramConfiguration
        {
            Buckets = Histogram.LinearBuckets(start: 0.001, width: 0.005, count: 20)
        });

    private static readonly Counter QueryCount = Metrics.CreateCounter(
        "db_query_total",
        "Total number of database queries executed");

    /// <inheritdoc/>
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        QueryCount.Inc();
        return base.ReaderExecuting(command, eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        QueryCount.Inc();
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    /// <inheritdoc/>
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        QueryDuration.Observe(eventData.Duration.TotalSeconds);
        return base.ReaderExecuted(command, eventData, result);
    }

    /// <inheritdoc/>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        QueryDuration.Observe(eventData.Duration.TotalSeconds);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }
}
