using Microsoft.EntityFrameworkCore;

namespace BasedGram.Database.Context;

public interface IDbContextFactory
{
    DbContext GetDbContext();
}