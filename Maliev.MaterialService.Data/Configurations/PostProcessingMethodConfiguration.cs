using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Data.Configurations;

public class PostProcessingMethodConfiguration : IEntityTypeConfiguration<PostProcessingMethod>
{
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
