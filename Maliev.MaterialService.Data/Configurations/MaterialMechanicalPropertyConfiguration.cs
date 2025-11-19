using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maliev.MaterialService.Data.Configurations;

public class MaterialMechanicalPropertyConfiguration : IEntityTypeConfiguration<MaterialMechanicalProperty>
{
    public void Configure(EntityTypeBuilder<MaterialMechanicalProperty> builder)
    {
        builder.HasKey(mmp => new { mmp.MaterialId, mmp.MechanicalPropertyId });

        builder.Property(mmp => mmp.Value)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.HasOne(mmp => mmp.Material)
            .WithMany(m => m.MechanicalProperties)
            .HasForeignKey(mmp => mmp.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mmp => mmp.MechanicalProperty)
            .WithMany(mp => mp.MaterialMechanicalProperties)
            .HasForeignKey(mmp => mmp.MechanicalPropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
