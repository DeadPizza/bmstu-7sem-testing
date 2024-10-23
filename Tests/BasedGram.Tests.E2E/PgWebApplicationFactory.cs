using System.Text;
using BasedGram.Database.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BasedGram.Tests.E2E;

internal class Settings
{
    public Settings() { }

    public string SymmetricFuncTestKey = "";
}

public class PgWebApplicationFactory<T> : WebApplicationFactory<T>
    where T : class
{
    private const string ConnectionString =
        @"Host=localhost;Port=7432;Username=postgres;Password=postgres;Database=testdb;Include Error Detail=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var settings = new Settings
        {
            SymmetricFuncTestKey =
                "my-32-character-ultra-secure-and-ultra-long-secret",
        };

        builder
            .UseEnvironment("Testing")
            .ConfigureTestServices(services =>
            {
                var options = new DbContextOptionsBuilder<BasedGramNpgsqlDbContext>()
                    .UseNpgsql(ConnectionString)
                    .EnableSensitiveDataLogging()
                    .Options;

                services.AddScoped<BasedGramNpgsqlDbContext>(
                    provider => new BasedGramTestContext(options)
                );

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedService = scope.ServiceProvider;
                var db = scopedService.GetRequiredService<BasedGramNpgsqlDbContext>();
                db.Database.EnsureCreated();
            });
    }
}