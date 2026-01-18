using Maliev.MaterialService.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maliev.MaterialService.Data;

public class MaterialDbContextFactory : IDesignTimeDbContextFactory<MaterialDbContext>
{
    public MaterialDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=material_app_db;Username=postgres;Password=password");

        return new MaterialDbContext(optionsBuilder.Options, null);
    }
}
