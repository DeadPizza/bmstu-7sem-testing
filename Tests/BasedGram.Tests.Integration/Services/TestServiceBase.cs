using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Database.Core.Repositories;
using BasedGram.Database.Npgsql.Models.Converters;
using BasedGram.Database.Npgsql.Models;
using BasedGram.Tests.Common.Factories.Db;
using Moq;
using BasedGram.Database.NpgsqlRepositories;

namespace BasedGram.Tests.Integration.Services;

public class TestServiceBase(DatabaseFixture dbFixture) : IAsyncLifetime
{
    protected readonly DatabaseFixture m_dbFixture = dbFixture;

    public UserRepository m_userRepository = new(dbFixture.CreateContext());
    public DialogRepository m_dialogRepository = new(dbFixture.CreateContext());
    public MessageRepository m_messageRepository = new(dbFixture.CreateContext());

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        m_dbFixture.Cleanup();
        m_userRepository = new(dbFixture.CreateContext());
        m_dialogRepository = new(dbFixture.CreateContext());
        m_messageRepository = new(dbFixture.CreateContext());
        return Task.CompletedTask;
    }

    protected List<User> m_users
    {
        get
        {
            return [.. (from a in m_dbFixture.CreateContext().Users select a).Select(u => UserConverter.DbToCoreModel(u))];
        }
        set
        {
            var context = m_dbFixture.CreateContext();
            foreach (var item in value)
            {
                context.Users.Add(UserConverter.CoreToDbModel(item));
                context.SaveChanges();
            }
        }
    }

    protected List<Dialog> m_dialogs
    {
        get
        {
            return [.. (from a in m_dbFixture.CreateContext().Dialogs select a).Select(u => DialogConverter.DbToCoreModel(u))];
        }
        set
        {
            var context = m_dbFixture.CreateContext();
            foreach (var item in value)
            {
                context.Dialogs.Add(DialogConverter.CoreToDbModel(item));
                context.SaveChanges();
            }
        }
    }

    protected List<Message> m_messages
    {
        get
        {
            return [.. (from a in m_dbFixture.CreateContext().Messages select a).Select(u => MessageConverter.DbToCoreModel(u))];
        }
        set
        {
            var context = m_dbFixture.CreateContext();
            foreach (var item in value)
            {
                context.Messages.Add(MessageConverter.CoreToDbModel(item));
                context.SaveChanges();
            }
        }
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
