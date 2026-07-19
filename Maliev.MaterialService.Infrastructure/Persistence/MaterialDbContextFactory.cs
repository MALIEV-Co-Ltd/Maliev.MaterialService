using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maliev.MaterialService.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating <see cref="MaterialDbContext"/> during EF Core migrations.
/// </summary>
public class MaterialDbContextFactory : IDesignTimeDbContextFactory<MaterialDbContext>
{
    /// <inheritdoc/>
    public MaterialDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=material_app_db;Username=postgres;Password=password");

        return new MaterialDbContext(optionsBuilder.Options, null);
    }
}
