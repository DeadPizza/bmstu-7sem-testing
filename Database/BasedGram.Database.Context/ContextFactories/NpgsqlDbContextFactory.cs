using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BasedGram.Database.Context;

public class NpgsqlDbContextFactory(IConfiguration config) : IDbContextFactory
{
    private readonly IConfiguration _config = config;
    public DbContext GetDbContext()
    {
        var connName = _config["Database Connection"]!;

        var builder = new DbContextOptionsBuilder<BasedGramNpgsqlDbContext>();
        builder.UseNpgsql(_config.GetConnectionString(connName));

        return new(builder.Options);
    }
}