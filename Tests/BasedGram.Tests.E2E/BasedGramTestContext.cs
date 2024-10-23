using BasedGram.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace BasedGram.Tests.E2E;

public class BasedGramTestContext : BasedGramNpgsqlDbContext
{
    public BasedGramTestContext(DbContextOptions<BasedGramNpgsqlDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
