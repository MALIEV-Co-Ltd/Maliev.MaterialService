using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Material"/> entity.
/// </summary>
public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(m => m.Name)
            .IsUnique();

        builder.Property(m => m.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => m.Code)
            .IsUnique();

        // Performance Indexes
        builder.HasIndex(m => m.PricePerUnit);
        builder.HasIndex(m => m.Active);
        builder.HasIndex(m => m.CreatedAt);

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.PricePerUnit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(m => m.StockLevel)
            .IsRequired();

        builder.HasOne(m => m.Supplier)
            .WithMany(s => s.Materials)
            .HasForeignKey(m => m.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(m => m.ManufacturingProcesses)
            .WithMany(mp => mp.Materials)
            .UsingEntity(j => j.ToTable("material_manufacturing_processes"));

        builder.HasMany(m => m.AvailableColors)
            .WithMany(c => c.Materials)
            .UsingEntity(j => j.ToTable("material_colors"));

        builder.HasMany(m => m.PostProcessingMethods)
            .WithMany(ppm => ppm.Materials)
            .UsingEntity(j => j.ToTable("material_post_processing_methods"));

        builder.HasMany(m => m.MechanicalProperties)
            .WithOne(mmp => mmp.Material)
            .HasForeignKey(mmp => mmp.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
