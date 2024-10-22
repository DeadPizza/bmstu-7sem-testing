using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Auth;

public class LogoutCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<LogoutCommand>();
    public override async Task Execute(Context ctx)
    {
        m_logger.Information("Logout user {@User}", new {ctx.User.ID});
        ctx.User = null;
    }

    public override string GetDescription()
    {
        return "Выйти";
    }
}