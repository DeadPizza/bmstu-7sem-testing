using BasedGram.Common.Core;
using BasedGram.Common.Enums;
using BasedGram.Database.Context;
using BasedGram.Database.Core.Repositories;
using BasedGram.Database.Npgsql.Models.Converters;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace BasedGram.Database.NpgsqlRepositories;

public class UserRepository(BasedGramNpgsqlDbContext context) : IUserRepository
{
    private readonly BasedGramNpgsqlDbContext m_context = context;
    private readonly ILogger m_logger = Log.ForContext<UserRepository>();
    public async Task CreateUser(User user)
    {
        m_logger.Verbose("CreateUser() enter");
        await m_context.Users.AddAsync(UserConverter.CoreToDbModel(user));
        await m_context.SaveChangesAsync();
        m_logger.Verbose("CreateUser() exit");
    }

    public async Task DeleteUser(User user)
    {
        m_logger.Verbose("DeleteUser() enter");
        var foundDbModel = await m_context.Users.FindAsync(user.ID);
        if (foundDbModel is not null)
        {
            m_context.Users.Remove(foundDbModel!);
            await m_context.SaveChangesAsync();
        }
        m_logger.Verbose("DeleteUser() exit");
    }

    public async Task<User?> GetUserByID(Guid guid)
    {
        m_logger.Verbose("GetUserByID() enter");
        var userDbModel = await m_context.Users.FindAsync(guid);
        m_logger.Verbose("GetUserByID() exit");
        return UserConverter.DbToCoreModel(userDbModel);
    }

    public Task<List<User>> ListAllAdmins()
    {
        m_logger.Verbose("ListAllAdmins()");
        return m_context.Users
            .Where(u => u.Role == 1)
            .Select(u => UserConverter.DbToCoreModel(u))
            .ToListAsync();
    }

    public Task<List<User>> ListAllUsers()
    {
        m_logger.Verbose("ListAllUsers()");
        return m_context.Users
            .Where(u => u.Role == 0)
            .Select(u => UserConverter.DbToCoreModel(u))
            .ToListAsync();
    }

    public async Task UpdateUser(User user)
    {
        m_logger.Verbose("UpdateUser() enter");
        var userDbModel = await m_context.Users.FindAsync(user.ID);
        if (userDbModel is not null)
        {
            userDbModel = UserConverter.CoreToDbModel(user);
            // m_context.Users.Update(UserConverter.CoreToDbModel(user));
        }
        else
        {
            await m_context.Users.AddAsync(UserConverter.CoreToDbModel(user));
        }
        await m_context.SaveChangesAsync();
        m_logger.Verbose("UpdateUser() exit");
    }
}