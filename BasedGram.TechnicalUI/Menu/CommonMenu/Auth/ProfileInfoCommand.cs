using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Auth;

public class ProfileInfoCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<ProfileInfoCommand>();
    public override async Task Execute(Context ctx)
    {
        m_logger.Information("Profile info for {@User}", new {ctx.User.ID, ctx.User.Login});
        
        Console.WriteLine($"ID: {ctx.User.ID}");
        Console.WriteLine($"Логин: {ctx.User.Login}");
        if (ctx.User.Role == Common.Enums.Role.Admin)
        {
            Console.WriteLine($"Статус администратора: является администратором");
        }
        if (ctx.User.Role == Common.Enums.Role.User)
        {
            Console.WriteLine($"Статус администратора: НЕ является администратором");
        }
    }

    public override string GetDescription()
    {
        return "Мой профиль";
    }
}