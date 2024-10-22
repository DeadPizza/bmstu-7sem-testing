using Microsoft.EntityFrameworkCore;

using BasedGram.Database;
using MongoDB.EntityFrameworkCore.Extensions;

namespace BasedGram.Database.Context;

public class BasedGramMongoDbContext(DbContextOptions<BasedGramMongoDbContext> options) : DbContext(options)
{
    public DbSet<MongoDB.Models.UserDbModel> Users { get; set; }
    public DbSet<MongoDB.Models.MessageDbModel> Messages { get; set; }
    public DbSet<MongoDB.Models.DialogDbModel> Dialogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MongoDB.Models.UserDbModel>().ToCollection("Users");
        modelBuilder.Entity<MongoDB.Models.MessageDbModel>().ToCollection("Messages");
        modelBuilder.Entity<MongoDB.Models.DialogDbModel>().ToCollection("Dialogs");
    }
}
