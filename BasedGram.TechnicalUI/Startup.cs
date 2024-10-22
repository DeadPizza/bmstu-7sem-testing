using Microsoft.Extensions.Configuration;
using BasedGram.TechnicalUI.MenuBase;
using BasedGram.Common.Enums;
using Microsoft.Extensions.Hosting.Internal;

namespace BasedGram.TechnicalUI;

internal class Startup(IConfiguration config,
               Context context,
               List<Menu> menus)
{
    private readonly IConfiguration _config = config;
    private Context _context = context;
    private readonly List<Menu> menus = menus;

    public async Task Run()
    {
        _context.UserService.SetCallbacks(_context.P2PService);
        _context.AuthService.SetCallbacks(_context.P2PService);
        _context.DialogService.SetCallbacks(_context.P2PService);

        _context.P2PService.RunNode();

        _context.Working = true;
        int cur_menu_index = 0;
        do
        {
            Console.WriteLine();
            if (_context.User is null)
            {
                cur_menu_index = 0;
            }
            else if (_context.User.Role == Role.User)
            {
                cur_menu_index = 1;
            }
            else if (_context.User.Role == Role.Admin)
            {
                cur_menu_index = 2;
            }
            menus[cur_menu_index].PrintDescription();
            await menus[cur_menu_index].Execute(_context);

        } while (_context.Working);
        System.Environment.Exit(0);
    }
}