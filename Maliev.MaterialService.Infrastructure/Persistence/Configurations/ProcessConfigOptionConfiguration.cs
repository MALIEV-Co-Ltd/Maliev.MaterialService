using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ProcessConfigOption"/> entity.
/// </summary>
public class ProcessConfigOptionConfiguration : IEntityTypeConfiguration<ProcessConfigOption>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ProcessConfigOption> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

        builder.Property(o => o.ConfigKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.ConfigType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.DefaultValue)
            .HasMaxLength(500);

        builder.Property(o => o.OptionsJson)
            .HasMaxLength(2000);

        builder.Property(o => o.Unit)
            .HasMaxLength(20);

        builder.Property(o => o.HelpText)
            .HasMaxLength(500);

        builder.Property(o => o.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Unique config key per process
        builder.HasIndex(o => new { o.ManufacturingProcessId, o.ConfigKey })
            .IsUnique();

        builder.HasIndex(o => o.Active);

        builder.HasOne(o => o.ManufacturingProcess)
            .WithMany(mp => mp.ConfigOptions)
            .HasForeignKey(o => o.ManufacturingProcessId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
