using BasedGram.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace BasedGram.Tests.Integration;
public class DatabaseFixture
{
    private string ConnectionString =
        $"Host=localhost;Port=7432;Username=postgres;Password=postgres;Database=testdb;Include Error Detail=true";

    public BasedGramNpgsqlDbContext CreateContext() =>
        new(
            new DbContextOptionsBuilder<BasedGramNpgsqlDbContext>()
                .UseNpgsql(ConnectionString)
                .EnableSensitiveDataLogging()
                .Options
        );

    public DatabaseFixture()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Cleanup();
    }

    public void Cleanup()
    {
        using var context = CreateContext();

        context.Users.RemoveRange(context.Users);
        context.Dialogs.RemoveRange(context.Dialogs);
        context.Messages.RemoveRange(context.Messages);

        // context.Users.RemoveRange(context.Users);
        // context.Playlists.RemoveRange(context.Playlists);
        // context.UsersFavourites.RemoveRange(context.UsersFavourites);
        context.SaveChanges();
    }
}

[CollectionDefinition("Test Database", DisableParallelization = true)]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }