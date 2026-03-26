using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ToleranceClass"/> entity.
/// </summary>
public class ToleranceClassConfiguration : IEntityTypeConfiguration<ToleranceClass>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ToleranceClass> builder)
    {
        builder.HasKey(tc => tc.Id);

        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Property(tc => tc.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tc => tc.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(tc => tc.Code)
            .IsUnique();

        builder.Property(tc => tc.IsoStandard)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(tc => tc.Grade)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(tc => tc.ToleranceRange)
            .HasMaxLength(200);

        builder.Property(tc => tc.AdditionalCostPercent)
            .HasPrecision(6, 2)
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(tc => tc.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasIndex(tc => tc.Active);
        builder.HasIndex(tc => tc.SortOrder);

        builder.HasMany(tc => tc.AvailableForProcesses)
            .WithMany(mp => mp.ToleranceClasses)
            .UsingEntity(j => j.ToTable("tolerance_class_processes"));
    }
}
