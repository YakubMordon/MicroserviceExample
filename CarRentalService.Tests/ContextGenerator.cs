namespace CarRentalService.Tests;

using Microsoft.EntityFrameworkCore;
using Data;

public static class ContextGenerator
{
    public static CarRentalDbContext Generate()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new CarRentalDbContext();
    }
}