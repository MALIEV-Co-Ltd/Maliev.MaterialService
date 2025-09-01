using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Maliev.MaterialService.Data.Models; // Add this using statement

namespace Maliev.MaterialService.Data
{
    public partial class MaterialContext : DbContext
    {
        public MaterialContext()
        {
        }

        public MaterialContext(DbContextOptions<MaterialContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Color> Color { get; set; }
        public virtual DbSet<Material> Material { get; set; }
        public virtual DbSet<MaterialGroup> MaterialGroup { get; set; }
        public virtual DbSet<MaterialHasColor> MaterialHasColor { get; set; }
        public virtual DbSet<MaterialHasSupplier> MaterialHasSupplier { get; set; }
        public virtual DbSet<MaterialHasSurfaceFinish> MaterialHasSurfaceFinish { get; set; }
        public virtual DbSet<SurfaceFinish> SurfaceFinish { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.x.xxxx"); // Updated ProductVersion

            modelBuilder.Entity<Color>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Afnor)
                    .HasColumnName("AFNOR")
                    .HasMaxLength(50);

                entity.Property(e => e.Aisi)
                    .HasColumnName("AISI")
                    .HasMaxLength(50);

                entity.Property(e => e.Ams)
                    .HasColumnName("AMS")
                    .HasMaxLength(50);

                entity.Property(e => e.Astm)
                    .HasColumnName("ASTM")
                    .HasMaxLength(50);

                entity.Property(e => e.Bts)
                    .HasColumnName("BTS")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");

                entity.Property(e => e.DensityKilogramPerCubicMeter).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.Din)
                    .HasColumnName("DIN")
                    .HasMaxLength(50);

                entity.Property(e => e.En)
                    .HasColumnName("EN")
                    .HasMaxLength(50);

                entity.Property(e => e.HardnessBrinell).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.HardnessKnoop).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.HardnessRockwellA).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.HardnessRockwellB).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.HardnessRockwellC).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.HardnessVickers).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.Jis)
                    .HasColumnName("JIS")
                    .HasMaxLength(50);

                entity.Property(e => e.MachinabilityPercent).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.ManufacturerReference).HasMaxLength(50);

                entity.Property(e => e.MaterialGroupId).HasColumnName("MaterialGroupID");

                entity.Property(e => e.MaterialNumber).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PricePerKilogram).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Sae)
                    .HasColumnName("SAE")
                    .HasMaxLength(50);

                entity.Property(e => e.ShearModulusGigaPascal).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.Sis)
                    .HasColumnName("SIS")
                    .HasMaxLength(50);

                entity.Property(e => e.TensileStrengthUltimateGigaPascal).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.TensileStrengthYieldMegaPascal).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.ThermalConductivityWattPerMeterKelvin).HasColumnType("decimal(7, 2)");

                entity.Property(e => e.Uni)
                    .HasColumnName("UNI")
                    .HasMaxLength(50);

                entity.Property(e => e.Uns)
                    .HasColumnName("UNS")
                    .HasMaxLength(50);

                entity.Property(e => e.Url).HasColumnName("URL");

                entity.HasOne(d => d.MaterialGroup)
                    .WithMany(p => p.Material)
                    .HasForeignKey(d => d.MaterialGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Material_MaterialGroup");
            });

            modelBuilder.Entity<MaterialGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<MaterialHasColor>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ColorId).HasColumnName("ColorID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.MaterialHasColor)
                    .HasForeignKey(d => d.ColorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialHasColor_Color");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialHasColor)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialHasColor_Material");
            });

            modelBuilder.Entity<MaterialHasSupplier>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialHasSupplier)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialHasSupplier_Material");
            });

            modelBuilder.Entity<MaterialHasSurfaceFinish>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.MaterialId).HasColumnName("MaterialID");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.SurfaceFinishId).HasColumnName("SurfaceFinishID");

                entity.HasOne(d => d.Material)
                    .WithMany(p => p.MaterialHasSurfaceFinish)
                    .HasForeignKey(d => d.MaterialId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialHasSurfaceFinish_Material");

                entity.HasOne(d => d.SurfaceFinish)
                    .WithMany(p => p.MaterialHasSurfaceFinish)
                    .HasForeignKey(d => d.SurfaceFinishId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MaterialHasSurfaceFinish_SurfaceFinish");
            });

            modelBuilder.Entity<SurfaceFinish>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}