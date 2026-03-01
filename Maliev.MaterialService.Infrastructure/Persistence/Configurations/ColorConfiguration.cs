using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Color"/> entity.
/// </summary>
public class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.HexCode)
            .HasMaxLength(7);

        builder.HasIndex(c => c.HexCode)
            .IsUnique();
    }
}
