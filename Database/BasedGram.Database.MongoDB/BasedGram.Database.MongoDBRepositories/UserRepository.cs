using BasedGram.Common.Core;
using BasedGram.Database.Context;
using BasedGram.Database.Core.Repositories;
using BasedGram.Database.MongoDB.Models.Converters;
using Microsoft.EntityFrameworkCore;

using Serilog;

namespace BasedGram.Database.MongoDBRepositories;

public class UserRepository(BasedGramMongoDbContext context) : IUserRepository
{
    private readonly BasedGramMongoDbContext _context = context;
    private readonly ILogger m_logger = Log.ForContext<UserRepository>();
    public async Task CreateUser(User user)
    {
        m_logger.Verbose("CreateUser() enter");
        await _context.Users.AddAsync(UserConverter.CoreToDbModel(user));
        await _context.SaveChangesAsync();
        m_logger.Verbose("CreateUser() exit");
    }

    public async Task DeleteUser(User user)
    {
        m_logger.Verbose("DeleteUser() enter");
        var foundDbModel = await _context.Users.FindAsync(user.ID);
        if (foundDbModel is not null)
        {
            _context.Remove(foundDbModel!);
            await _context.SaveChangesAsync();
        }
        m_logger.Verbose("DeleteUser() exit");
    }

    public async Task<User?> GetUserByID(Guid guid)
    {
        m_logger.Verbose("GetUserByID() enter");
        var userDbModel = await _context.Users.FindAsync(guid);
        m_logger.Verbose("GetUserByID() exit");
        return UserConverter.DbToCoreModel(userDbModel);
    }

    public async Task<List<User>> ListAllAdmins()
    {
        m_logger.Verbose("ListAllAdmins()");
        return (await _context.Users
            .Where(u => u.Role == 1)
            .ToListAsync())
            .Select(u => UserConverter.DbToCoreModel(u))
            .ToList();
    }

    public async Task<List<User>> ListAllUsers()
    {
        m_logger.Verbose("ListAllUsers()");
        return (await _context.Users
            .Where(u => u.Role == 0)
            .ToListAsync())
            .Select(u => UserConverter.DbToCoreModel(u))
            .ToList();
    }

    public async Task UpdateUser(User user)
    {
        m_logger.Verbose("UpdateUser() enter");
        var userDbModel = await _context.Users.FindAsync(user.ID);
        if (userDbModel is not null)
        {
            var fromUpd = UserConverter.CoreToDbModel(user);
            userDbModel.IsFreezed = fromUpd.IsFreezed;
            // m_context.Users.Update(UserConverter.CoreToDbModel(user));
        }
        else
        {
            await _context.Users.AddAsync(UserConverter.CoreToDbModel(user));
        }
        await _context.SaveChangesAsync();
        m_logger.Verbose("UpdateUser() exit");
    }
}
