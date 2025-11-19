using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Maliev.MaterialService.Data.DbContext;

namespace Maliev.MaterialService.Data;

public class MaterialDbContextFactory : IDesignTimeDbContextFactory<MaterialDbContext>
{
    public MaterialDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=material_app_db;Username=postgres;Password=password");
        optionsBuilder.UseSnakeCaseNamingConvention();

        return new MaterialDbContext(optionsBuilder.Options);
    }
}
