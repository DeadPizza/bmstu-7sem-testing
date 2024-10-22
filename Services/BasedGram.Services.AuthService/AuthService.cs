using BasedGram.Common.Core;
using BasedGram.Database.Core.Repositories;
using BasedGram.Services.AuthService;
using BasedGram.Services.P2PService;
using BasedGram.Services.AuthService.Exceptions;
using BCrypt;
using Serilog;

namespace BasedGram.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IUserRepository m_UserRepository;
    private readonly IP2PService m_p2pservice;
    private readonly ILogger m_logger = Log.ForContext<AuthService>();
    public AuthService(IUserRepository userRepository, IP2PService p2pservice)
    {
        m_UserRepository = userRepository;
        m_p2pservice = p2pservice;
    }
    public void SetCallbacks(IP2PService service)
    {
        service ??= m_p2pservice;

        m_logger.Verbose("SetCallbacks() enter");
        service.OnUserListReceive(async (List<User> users) =>
        {
            m_logger.Verbose("OnUserListReceive() enter");
            m_logger.Information($"Receiving {users.Count} users");

            var all_users = await m_UserRepository.ListAllUsers();
            all_users.AddRange(await m_UserRepository.ListAllAdmins());

            foreach (var user in users)
            {
                await m_UserRepository.UpdateUser(user);
            }
            m_logger.Verbose("OnUserListReceive() exit");
        });

        m_logger.Verbose("SetCallbacks() exit");
    }
    public async Task<User> LoginUser(string username, String password)
    {
        m_logger.Verbose("LoginUser() enter");
        var all_users = (await m_UserRepository.ListAllUsers());
        all_users.AddRange(await m_UserRepository.ListAllAdmins());

        var matching_user_accs = all_users.FindAll(u => u.Login == username);

        if (matching_user_accs.Count == 0)
        {
            m_logger.Error("User {@User} not found!", new { username });
            throw new UserLoginNotFoundException();
        }

        var usr = matching_user_accs.Find(u => BCrypt.Net.BCrypt.Verify(password, u.PasswordHash));
        if (usr != null)
        {
            m_logger.Information("User {@User} login successful!", new { usr.ID, usr.Login });
            m_logger.Verbose("LoginUser() exit");
            return usr;
        }

        m_logger.Error("Invalid password for User {@User}", new { username });
        throw new IncorrectPasswordException("Invalid password!");
    }
    public async Task RegisterUser(string username, String password)
    {
        m_logger.Verbose("RegisterUser() enter");
        var all_users = await m_UserRepository.ListAllUsers();

        var matching_user_accs = all_users.FindAll(u => u.Login == username && BCrypt.Net.BCrypt.Verify(password, u.PasswordHash));

        if (matching_user_accs.Count != 0)
        {
            m_logger.Error("User: {@User} already exists!", new { username });
            throw new UserRegisterAlreadyExistsException("User already exists!");
        }

        var new_user = new User(
            Guid.NewGuid(),
            username,
            BCrypt.Net.BCrypt.HashPassword(password)
            );

        m_logger.Information("Created new user: {@User}", new { new_user.ID, new_user.Login });
        // Console.WriteLine($"sending user list to {m_p2pservice}");
        if (m_p2pservice is not null)
        {
            await m_p2pservice.SendUserList([new_user]);
        }
        // Console.WriteLine("sending user list done");
        await m_UserRepository.CreateUser(new_user);
        m_logger.Verbose("RegisterUser() exit");
    }
}
