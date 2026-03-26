using Maliev.Aspire.ServiceDefaults.Database;
using Maliev.MaterialService.Domain.Entities;
using Maliev.MaterialService.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Maliev.MaterialService.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Material Service.
/// </summary>
public class MaterialDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly DatabaseMetricsInterceptor? _metricsInterceptor;

    /// <summary>
    /// Initializes a new instance of <see cref="MaterialDbContext"/>.
    /// </summary>
    /// <param name="options">EF Core options.</param>
    /// <param name="metricsInterceptor">Optional database metrics interceptor.</param>
    public MaterialDbContext(
        DbContextOptions<MaterialDbContext> options,
        DatabaseMetricsInterceptor? metricsInterceptor) : base(options)
    {
        _metricsInterceptor = metricsInterceptor;
    }

    /// <summary>Materials data set.</summary>
    public DbSet<Material> Materials => Set<Material>();

    /// <summary>Manufacturing processes data set.</summary>
    public DbSet<ManufacturingProcess> ManufacturingProcesses => Set<ManufacturingProcess>();

    /// <summary>Colors data set.</summary>
    public DbSet<Color> Colors => Set<Color>();

    /// <summary>Mechanical properties data set.</summary>
    public DbSet<MechanicalProperty> MechanicalProperties => Set<MechanicalProperty>();

    /// <summary>Post-processing methods data set.</summary>
    public DbSet<PostProcessingMethod> PostProcessingMethods => Set<PostProcessingMethod>();

    /// <summary>Suppliers data set.</summary>
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    /// <summary>Material mechanical properties join data set.</summary>
    public DbSet<MaterialMechanicalProperty> MaterialMechanicalProperties => Set<MaterialMechanicalProperty>();

    /// <summary>Surface finishes data set.</summary>
    public DbSet<SurfaceFinish> SurfaceFinishes => Set<SurfaceFinish>();

    /// <summary>Tolerance classes data set.</summary>
    public DbSet<ToleranceClass> ToleranceClasses => Set<ToleranceClass>();

    /// <summary>Process configuration options data set.</summary>
    public DbSet<ProcessConfigOption> ProcessConfigOptions => Set<ProcessConfigOption>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MaterialDbContext).Assembly);

        // Apply PostgreSQL snake_case naming convention globally
        SnakeCaseNamingHelper.ApplySnakeCaseNaming(modelBuilder);
    }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (_metricsInterceptor is not null)
        {
            optionsBuilder.AddInterceptors(_metricsInterceptor);
        }
    }

    /// <inheritdoc/>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <inheritdoc/>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(entry.Entity.CreatedBy)) entry.Entity.CreatedBy = "System";
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(entry.Entity.UpdatedBy)) entry.Entity.UpdatedBy = "System";
            }
        }
    }
}
