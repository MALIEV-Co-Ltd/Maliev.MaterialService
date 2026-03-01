using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ManufacturingProcess"/> entity.
/// </summary>
public class ManufacturingProcessConfiguration : IEntityTypeConfiguration<ManufacturingProcess>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<ManufacturingProcess> builder)
    {
        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(mp => mp.Name)
            .IsUnique();
    }
}
