using BasedGram.Common.Core;

using BasedGram.Services.P2PService;

namespace BasedGram.Services.AuthService;

public interface IAuthService
{
    void SetCallbacks(IP2PService service);
    Task RegisterUser(String username, String password);
    Task<User> LoginUser(String username, String password);
}