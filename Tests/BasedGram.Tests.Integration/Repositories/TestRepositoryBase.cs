using System.Collections;
using BasedGram.Common.Enums;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Tests.Common.Factories.Db;

namespace BasedGram.Tests.Integration.Repositories;

public class TestRepositoryBase(DatabaseFixture dbFixture) : IAsyncLifetime
{
    protected readonly DatabaseFixture m_dbFixture = dbFixture;

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        m_dbFixture.Cleanup();
        return Task.CompletedTask;
    }

    public Guid MakeGuid(byte val)
    {
        return new Guid([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, val]);
    }

    public async Task<UserDbModel> CreateSingleUser()
    {
        var context = m_dbFixture.CreateContext();

        var user_1 = UserDbModelFactory.Create(
            MakeGuid(1),
            "A",
            BCrypt.Net.BCrypt.HashPassword("Hash1"),
            0,
            true,
            false);
        await context.Users.AddAsync(user_1);
        await context.SaveChangesAsync();
        return user_1;
    }

    public async Task<UserDbModel> CreateSingleUserFrom(UserDbModel user_1)
    {
        var context = m_dbFixture.CreateContext();

        await context.Users.AddAsync(user_1);
        await context.SaveChangesAsync();
        return user_1;
    }

    public async Task<DialogDbModel> CreateSingleDialogFrom(DialogDbModel dialog)
    {
        var context = m_dbFixture.CreateContext();

        await context.Dialogs.AddAsync(dialog);
        await context.SaveChangesAsync();
        return dialog;
    }
    public async Task<MessageDbModel> CreateSingleMessageFrom(MessageDbModel message)
    {
        var context = m_dbFixture.CreateContext();

        await context.Messages.AddAsync(message);
        await context.SaveChangesAsync();
        return message;
    }

    public async Task<List<UserDbModel>> CreateDefaultUsers()
    {
        var context = m_dbFixture.CreateContext();

        var user_1 = UserDbModelFactory.Create(
                MakeGuid(1),
                "A",
                BCrypt.Net.BCrypt.HashPassword("Hash1"),
                0,
                true,
                false);
        await context.Users.AddAsync(user_1);

        var user_2 = UserDbModelFactory.Create(
                MakeGuid(2),
                "B",
                BCrypt.Net.BCrypt.HashPassword("Hash2"),
                0,
                true,
                false);
        await context.Users.AddAsync(user_2);

        await context.SaveChangesAsync();

        return [user_1, user_2];
    }
}
