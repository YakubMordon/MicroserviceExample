using Microsoft.EntityFrameworkCore;
using PaymentService.Data;

namespace PaymentService.Tests;

public static class ContextGenerator
{
    public static PaymentDbContext Generate()
    {
        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new PaymentDbContext(options);
    }
}