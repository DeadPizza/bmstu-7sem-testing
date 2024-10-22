using System.Formats.Tar;
using BasedGram.Common.Core;
using BasedGram.Database.Core.Repositories;
using BasedGram.Services.P2PService;
using BasedGram.Services.UserService.Exceptions;
using Serilog;

namespace BasedGram.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository m_UserRepository;
    private readonly IP2PService m_p2pservice;
    private readonly ILogger m_logger = Log.ForContext<UserService>();

    public UserService(IUserRepository userRepository, IP2PService p2pservice)
    {
        m_UserRepository = userRepository;
        m_p2pservice = p2pservice;
    }
    public void SetCallbacks(IP2PService service)
    {
        service ??= m_p2pservice;

        m_logger.Verbose("SetCallbacks() enter");
        service.OnSyncData(async () =>
        {
            m_logger.Verbose("OnSyncData() enter");
            var usr_li = await ListAllUsers();
            usr_li.AddRange(await m_UserRepository.ListAllAdmins());
            m_logger.Information($"Sending {usr_li.Count} users");
            await service.SendUserList(await ListAllUsers());
            m_logger.Verbose("OnSyncData() exit");
        });
        m_logger.Verbose("SetCallbacks() exit");
    }

    public async Task EraseUser(User user)
    {
        m_logger.Verbose("EraseUser() enter");
        await m_UserRepository.DeleteUser(user);
        m_logger.Verbose("User {@User} deleted", new { user.ID });
        m_logger.Verbose("EraseUser() exit");
    }

    public async Task FreezeUser(User user)
    {
        user.IsFreezed = !user.IsFreezed;
        await m_UserRepository.UpdateUser(user);
    }

    public async Task<List<User>> FindUser(string name)
    {
        m_logger.Verbose("FindUser() enter");
        var user_list = await m_UserRepository.ListAllUsers();
        user_list.AddRange(await m_UserRepository.ListAllAdmins());
        user_list.RemoveAll(Usr => !Usr.Login.Contains(name));
        m_logger.Verbose("FindUser() exit");
        return user_list;
    }

    public async Task<User> GetUser(Guid guid)
    {
        m_logger.Verbose("GetUser() enter");
        try
        {
            var user = await m_UserRepository.GetUserByID(guid);
            if(user is null)
            {
                throw new Exception();
            }
            m_logger.Verbose("GetUser() exit");
            return user;
        }
        catch (Exception)
        {
            throw new UserNotFoundException();
        }
    }

    public async Task<List<User>> ListAllUsers()
    {
        m_logger.Verbose("ListAllUsers() enter");
        try
        {
            var user_list = await m_UserRepository.ListAllUsers();
            user_list.AddRange(await m_UserRepository.ListAllAdmins());
            // user_list. await m_UserRepository.Li();
            m_logger.Verbose("ListAllUsers() exit");
            return user_list;
        }
        catch (Exception)
        {
            throw;
        }
    }
}