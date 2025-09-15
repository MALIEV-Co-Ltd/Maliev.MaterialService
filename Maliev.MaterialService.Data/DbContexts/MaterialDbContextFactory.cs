using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Maliev.MaterialService.Data.DbContexts;

public class MaterialDbContextFactory : IDesignTimeDbContextFactory<MaterialDbContext>
{
    public MaterialDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();
        // Use in-memory database for design-time operations
        optionsBuilder.UseInMemoryDatabase("MaterialDbContext");

        return new MaterialDbContext(optionsBuilder.Options);
    }
}