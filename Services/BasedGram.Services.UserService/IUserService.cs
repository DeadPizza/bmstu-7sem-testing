using BasedGram.Common.Core;
using BasedGram.Services.P2PService;

namespace BasedGram.Services.UserService;

public interface IUserService
{
    void SetCallbacks(IP2PService service);
    Task<User> GetUser(Guid guid);
    Task EraseUser(User user);
    Task FreezeUser(User user);
    Task<List<User>> ListAllUsers();
    Task<List<User>> FindUser(String name);
}