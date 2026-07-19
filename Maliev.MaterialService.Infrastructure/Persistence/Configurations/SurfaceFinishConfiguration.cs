using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="SurfaceFinish"/> entity.
/// </summary>
public class SurfaceFinishConfiguration : IEntityTypeConfiguration<SurfaceFinish>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<SurfaceFinish> builder)
    {
        builder.HasKey(sf => sf.Id);

        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Property(sf => sf.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sf => sf.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(sf => sf.Code)
            .IsUnique();

        builder.Property(sf => sf.Description)
            .HasMaxLength(500);

        builder.Property(sf => sf.RaValueUm)
            .HasPrecision(8, 4);

        builder.Property(sf => sf.AdditionalCostPercent)
            .HasPrecision(6, 2)
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(sf => sf.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(sf => sf.Active);
        builder.HasIndex(sf => sf.SortOrder);

        builder.HasMany(sf => sf.AvailableForProcesses)
            .WithMany(mp => mp.SurfaceFinishes)
            .UsingEntity(j => j.ToTable("surface_finish_processes"));
    }
}
