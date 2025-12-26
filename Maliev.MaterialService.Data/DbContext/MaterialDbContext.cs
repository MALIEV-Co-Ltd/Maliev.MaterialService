using Microsoft.EntityFrameworkCore;
using Maliev.Aspire.ServiceDefaults.Database;
using Maliev.MaterialService.Data.Entities;
using System.Threading;
using System.Threading.Tasks;
using System;
using Maliev.MaterialService.Data.Interceptors;

namespace Maliev.MaterialService.Data.DbContext;

public class MaterialDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly DatabaseMetricsInterceptor? _metricsInterceptor;

    public MaterialDbContext(
        DbContextOptions<MaterialDbContext> options,
        DatabaseMetricsInterceptor? metricsInterceptor) : base(options)
    {
        _metricsInterceptor = metricsInterceptor;
    }

    // DbSets
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<ManufacturingProcess> ManufacturingProcesses => Set<ManufacturingProcess>();
    public DbSet<Color> Colors => Set<Color>();
    public DbSet<MechanicalProperty> MechanicalProperties => Set<MechanicalProperty>();
    public DbSet<PostProcessingMethod> PostProcessingMethods => Set<PostProcessingMethod>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<MaterialMechanicalProperty> MaterialMechanicalProperties => Set<MaterialMechanicalProperty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MaterialDbContext).Assembly);

        // Apply PostgreSQL snake_case naming convention globally
        SnakeCaseNamingHelper.ApplySnakeCaseNaming(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (_metricsInterceptor is not null)
        {
            optionsBuilder.AddInterceptors(_metricsInterceptor);

        }
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

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
                entry.Entity.Version = 1;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                if (string.IsNullOrEmpty(entry.Entity.UpdatedBy)) entry.Entity.UpdatedBy = "System";
                entry.Entity.Version++;
            }
        }
    }
}
