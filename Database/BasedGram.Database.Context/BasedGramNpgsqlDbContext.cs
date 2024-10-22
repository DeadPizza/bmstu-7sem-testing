using Microsoft.EntityFrameworkCore;

using BasedGram.Database;

namespace BasedGram.Database.Context;

public class BasedGramNpgsqlDbContext : DbContext
{
    public BasedGramNpgsqlDbContext(DbContextOptions<BasedGramNpgsqlDbContext> options) : base(options)
    {
    }

    protected BasedGramNpgsqlDbContext(): base()
    {
    }

    public virtual DbSet<Npgsql.Models.UserDbModel> Users { get; set; }
    public virtual DbSet<Npgsql.Models.MessageDbModel> Messages { get; set; }
    public virtual DbSet<Npgsql.Models.DialogDbModel> Dialogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Npgsql.Models.DialogDbModel>()
            .HasAlternateKey(k => new {k.CreatorID, k.ColocutorID});
            
        base.OnModelCreating(modelBuilder);
    }
}
