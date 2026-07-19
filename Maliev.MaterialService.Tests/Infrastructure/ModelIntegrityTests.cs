using Maliev.MaterialService.Infrastructure.Persistence;
using Maliev.MaterialService.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Xunit;

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

        Assert.False(hasChanges, "Run 'dotnet ef migrations add <Name> --project Maliev.MaterialService.Infrastructure --startup-project Maliev.MaterialService.Api'");
    }
}
