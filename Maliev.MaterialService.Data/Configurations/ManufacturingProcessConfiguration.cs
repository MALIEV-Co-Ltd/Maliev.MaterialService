using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Data.Configurations;

public class ManufacturingProcessConfiguration : IEntityTypeConfiguration<ManufacturingProcess>
{
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
