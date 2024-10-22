using Moq;
using Microsoft.EntityFrameworkCore;
using BasedGram.Database.Context;
using BasedGram.Database.Npgsql.Models;


namespace BasedGram.Tests.Unit;
public class MockDbContextFactory
{
    public Mock<BasedGramNpgsqlDbContext> MockContext { get; set; }

    public Mock<DbSet<UserDbModel>> MockUsers { get; set; }
    public Mock<DbSet<MessageDbModel>> MockMessages { get; set; }
    public Mock<DbSet<DialogDbModel>> MockDialogs { get; set; }

    public MockDbContextFactory()
    {
        MockContext = new Mock<BasedGramNpgsqlDbContext>();

        MockUsers = SetupMockDbSet(new List<UserDbModel>());
        MockMessages = SetupMockDbSet(new List<MessageDbModel>());
        MockDialogs = SetupMockDbSet(new List<DialogDbModel>());

        MockContext
            .Setup(u => u.Users)
            .Returns(MockUsers.Object);

        MockContext
            .Setup(u => u.Messages)
            .Returns(MockMessages.Object);

        MockContext
            .Setup(u => u.Dialogs)
            .Returns(MockDialogs.Object);
    }

    public static Mock<DbSet<T>> SetupMockDbSet<T>(List<T> list)
    where T : class
    {
        var queryable = list.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(queryable.Provider);
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());
        return mockDbSet;
    }
}