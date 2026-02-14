using Maliev.MaterialService.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Maliev.MaterialService.Data.Interceptors;

namespace Maliev.MaterialService.Tests.Infrastructure;

public class ModelIntegrityTests
{
    [Fact]
    public void Model_ShouldNotHavePendingChanges()
    {
        var options = new DbContextOptionsBuilder<MaterialDbContext>()
            .UseNpgsql("Host=localhost;Database=ModelCheck")
            .Options;

        using var context = new MaterialDbContext(options, null);
        var hasChanges = context.Database.HasPendingModelChanges();

        Assert.False(hasChanges, "Run 'dotnet ef migrations add <Name> --project Maliev.MaterialService.Data --startup-project Maliev.MaterialService.Api'");
    }
}
