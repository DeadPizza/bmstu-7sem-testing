using BasedGram.Common.Core;
using BasedGram.Services.AuthService;
using BasedGram.Services.DialogService;
using BasedGram.Services.P2PService;
using BasedGram.Services.UserService;

namespace BasedGram.TechnicalUI.MenuBase;

public class Context(AuthService auth, DialogService dialog, P2PService p2p, UserService usr)
{
    public IAuthService AuthService { get; } = auth;
    public IDialogService DialogService { get; } = dialog;
    public IP2PService P2PService { get; } = p2p;
    public IUserService UserService { get; } = usr;

    public User? User { get; set; }
    public Object? Object { get; set; }
    public bool Working { get; set; }
}