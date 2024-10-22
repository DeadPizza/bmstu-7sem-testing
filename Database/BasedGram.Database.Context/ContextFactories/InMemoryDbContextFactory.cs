using Microsoft.EntityFrameworkCore;

namespace BasedGram.Database.Context;

public class InMemoryDbContextFactory : IDbContextFactory
{
    private readonly string _dbName = $"BasedGramTestDb_{Guid.NewGuid()}";
    public DbContext GetDbContext()
    {
        var builder = new DbContextOptionsBuilder<BasedGramNpgsqlDbContext>();
        builder.UseInMemoryDatabase(_dbName);

        return new(builder.Options);
    }
}