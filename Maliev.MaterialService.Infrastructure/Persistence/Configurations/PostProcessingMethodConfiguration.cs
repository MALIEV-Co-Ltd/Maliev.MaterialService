using Maliev.MaterialService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="PostProcessingMethod"/> entity.
/// </summary>
public class PostProcessingMethodConfiguration : IEntityTypeConfiguration<PostProcessingMethod>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PostProcessingMethod> builder)
    {
        builder.HasKey(ppm => ppm.Id);

        builder.Property(ppm => ppm.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(ppm => ppm.Name)
            .IsUnique();
    }
}
