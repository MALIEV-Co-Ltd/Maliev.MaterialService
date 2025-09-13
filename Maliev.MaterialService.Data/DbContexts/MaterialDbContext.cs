using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Maliev.MaterialService.Data.DbContexts;

public class MaterialDbContext : DbContext
{
    public MaterialDbContext(DbContextOptions<MaterialDbContext> options) : base(options)
    {
    }

    public DbSet<MaterialFamily> MaterialFamilies { get; set; }
    public DbSet<MaterialGroup> MaterialGroups { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<ManufacturingProcessCategory> ManufacturingProcessCategories { get; set; }
    public DbSet<ManufacturingProcess> ManufacturingProcesses { get; set; }
    public DbSet<MaterialStandardType> MaterialStandardTypes { get; set; }
    public DbSet<MaterialStandard> MaterialStandards { get; set; }
    public DbSet<PropertyType> PropertyTypes { get; set; }
    public DbSet<PropertySubtype> PropertySubtypes { get; set; }
    public DbSet<MaterialProperty> MaterialProperties { get; set; }
    public DbSet<MaterialProcessCompatibility> MaterialProcessCompatibilities { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<MaterialColor> MaterialColors { get; set; }
    public DbSet<SurfaceFinish> SurfaceFinishes { get; set; }
    public DbSet<MaterialSurfaceFinish> MaterialSurfaceFinishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names
        modelBuilder.Entity<MaterialFamily>().ToTable("MaterialFamilies");
        modelBuilder.Entity<MaterialGroup>().ToTable("MaterialGroups");
        modelBuilder.Entity<Material>().ToTable("Materials");
        modelBuilder.Entity<ManufacturingProcessCategory>().ToTable("ManufacturingProcessCategories");
        modelBuilder.Entity<ManufacturingProcess>().ToTable("ManufacturingProcesses");
        modelBuilder.Entity<MaterialStandardType>().ToTable("MaterialStandardTypes");
        modelBuilder.Entity<MaterialStandard>().ToTable("MaterialStandards");
        modelBuilder.Entity<PropertyType>().ToTable("PropertyTypes");
        modelBuilder.Entity<PropertySubtype>().ToTable("PropertySubtypes");
        modelBuilder.Entity<MaterialProperty>().ToTable("MaterialProperties");
        modelBuilder.Entity<MaterialProcessCompatibility>().ToTable("MaterialProcessCompatibilities");
        modelBuilder.Entity<Color>().ToTable("Colors");
        modelBuilder.Entity<MaterialColor>().ToTable("MaterialColors");
        modelBuilder.Entity<SurfaceFinish>().ToTable("SurfaceFinishes");
        modelBuilder.Entity<MaterialSurfaceFinish>().ToTable("MaterialSurfaceFinishes");

        // Configure relationships
        modelBuilder.Entity<MaterialGroup>()
            .HasOne(mg => mg.MaterialFamily)
            .WithMany(mf => mf.MaterialGroups)
            .HasForeignKey(mg => mg.MaterialFamilyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Material>()
            .HasOne(m => m.MaterialGroup)
            .WithMany(mg => mg.Materials)
            .HasForeignKey(m => m.MaterialGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ManufacturingProcess>()
            .HasOne(mp => mp.Category)
            .WithMany(mpc => mpc.ManufacturingProcesses)
            .HasForeignKey(mp => mp.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaterialStandard>()
            .HasOne(ms => ms.Material)
            .WithMany(m => m.MaterialStandards)
            .HasForeignKey(ms => ms.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaterialStandard>()
            .HasOne(ms => ms.StandardType)
            .WithMany(mst => mst.MaterialStandards)
            .HasForeignKey(ms => ms.StandardTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PropertySubtype>()
            .HasOne(ps => ps.PropertyType)
            .WithMany(pt => pt.PropertySubtypes)
            .HasForeignKey(ps => ps.PropertyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaterialProperty>()
            .HasOne(mp => mp.Material)
            .WithMany(m => m.MaterialProperties)
            .HasForeignKey(mp => mp.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaterialProperty>()
            .HasOne(mp => mp.PropertySubtype)
            .WithMany(ps => ps.MaterialProperties)
            .HasForeignKey(mp => mp.PropertySubtypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaterialProcessCompatibility>()
            .HasOne(mpc => mpc.Material)
            .WithMany(m => m.ProcessCompatibilities)
            .HasForeignKey(mpc => mpc.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaterialProcessCompatibility>()
            .HasOne(mpc => mpc.Process)
            .WithMany(mp => mp.MaterialCompatibilities)
            .HasForeignKey(mpc => mpc.ProcessId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaterialColor>()
            .HasOne(mc => mc.Material)
            .WithMany(m => m.MaterialColors)
            .HasForeignKey(mc => mc.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaterialColor>()
            .HasOne(mc => mc.Color)
            .WithMany(c => c.MaterialColors)
            .HasForeignKey(mc => mc.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MaterialSurfaceFinish>()
            .HasOne(msf => msf.Material)
            .WithMany(m => m.MaterialSurfaceFinishes)
            .HasForeignKey(msf => msf.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaterialSurfaceFinish>()
            .HasOne(msf => msf.SurfaceFinish)
            .WithMany(sf => sf.MaterialSurfaceFinishes)
            .HasForeignKey(msf => msf.SurfaceFinishId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure indexes for performance
        modelBuilder.Entity<Material>()
            .HasIndex(m => m.Name)
            .HasDatabaseName("IX_Materials_Name");

        modelBuilder.Entity<Material>()
            .HasIndex(m => m.MaterialGroupId)
            .HasDatabaseName("IX_Materials_MaterialGroupId");

        modelBuilder.Entity<MaterialStandard>()
            .HasIndex(ms => ms.StandardValue)
            .HasDatabaseName("IX_MaterialStandards_StandardValue");

        modelBuilder.Entity<MaterialStandard>()
            .HasIndex(ms => new { ms.MaterialId, ms.StandardTypeId })
            .HasDatabaseName("IX_MaterialStandards_MaterialId_StandardTypeId");

        modelBuilder.Entity<MaterialProcessCompatibility>()
            .HasIndex(mpc => mpc.MaterialId)
            .HasDatabaseName("IX_MaterialProcessCompatibilities_MaterialId");

        modelBuilder.Entity<MaterialProcessCompatibility>()
            .HasIndex(mpc => mpc.ProcessId)
            .HasDatabaseName("IX_MaterialProcessCompatibilities_ProcessId");

        modelBuilder.Entity<MaterialProperty>()
            .HasIndex(mp => new { mp.MaterialId, mp.PropertySubtypeId })
            .HasDatabaseName("IX_MaterialProperties_MaterialId_PropertySubtypeId");

        // Configure unique constraints
        modelBuilder.Entity<MaterialStandardType>()
            .HasIndex(mst => mst.Code)
            .IsUnique()
            .HasDatabaseName("IX_MaterialStandardTypes_Code_Unique");

        modelBuilder.Entity<MaterialStandard>()
            .HasIndex(ms => new { ms.MaterialId, ms.StandardTypeId })
            .IsUnique()
            .HasDatabaseName("IX_MaterialStandards_MaterialId_StandardTypeId_Unique");

        modelBuilder.Entity<MaterialColor>()
            .HasIndex(mc => new { mc.MaterialId, mc.ColorId })
            .IsUnique()
            .HasDatabaseName("IX_MaterialColors_MaterialId_ColorId_Unique");

        modelBuilder.Entity<MaterialSurfaceFinish>()
            .HasIndex(msf => new { msf.MaterialId, msf.SurfaceFinishId })
            .IsUnique()
            .HasDatabaseName("IX_MaterialSurfaceFinishes_MaterialId_SurfaceFinishId_Unique");

        // Configure default values for timestamps
        modelBuilder.Entity<MaterialFamily>()
            .Property(mf => mf.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MaterialFamily>()
            .Property(mf => mf.ModifiedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Apply similar timestamp defaults to all entities
        var timestampEntities = new[]
        {
            typeof(MaterialGroup), typeof(Material), typeof(ManufacturingProcessCategory),
            typeof(ManufacturingProcess), typeof(MaterialStandardType), typeof(MaterialStandard),
            typeof(PropertyType), typeof(PropertySubtype), typeof(MaterialProperty),
            typeof(MaterialProcessCompatibility), typeof(Color), typeof(MaterialColor),
            typeof(SurfaceFinish), typeof(MaterialSurfaceFinish)
        };

        foreach (var entityType in timestampEntities)
        {
            modelBuilder.Entity(entityType)
                .Property("CreatedDate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity(entityType)
                .Property("ModifiedDate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity.GetType().GetProperty("ModifiedDate") != null)
            {
                entry.Property("ModifiedDate").CurrentValue = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added && entry.Entity.GetType().GetProperty("CreatedDate") != null)
            {
                entry.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}