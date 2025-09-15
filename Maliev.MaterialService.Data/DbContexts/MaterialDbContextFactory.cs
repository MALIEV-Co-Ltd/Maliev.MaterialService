using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Maliev.MaterialService.Data.DbContexts;

public class MaterialDbContextFactory : IDesignTimeDbContextFactory<MaterialDbContext>
{
    public MaterialDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MaterialDbContext>();

        // Get connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MaterialDbContext");

        // If not found, throw an exception as connection string is required
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings__MaterialDbContext environment variable is required for database operations.");
        }

        optionsBuilder.UseNpgsql(connectionString);

        return new MaterialDbContext(optionsBuilder.Options);
    }
}