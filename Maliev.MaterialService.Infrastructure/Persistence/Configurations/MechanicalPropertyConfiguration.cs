using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="MechanicalProperty"/> entity.
/// </summary>
public class MechanicalPropertyConfiguration : IEntityTypeConfiguration<MechanicalProperty>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<MechanicalProperty> builder)
    {
        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(mp => mp.Name)
            .IsUnique();

        builder.Property(mp => mp.Unit)
            .IsRequired()
            .HasMaxLength(50);
    }
}
