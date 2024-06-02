namespace AuthenticationService.Tests;

using Microsoft.EntityFrameworkCore;
using Data;

public static class ContextGenerator
{
    public static AuthenticationDbContext Generate()
    {
        var options = new DbContextOptionsBuilder<AuthenticationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AuthenticationDbContext(options);
    }
}