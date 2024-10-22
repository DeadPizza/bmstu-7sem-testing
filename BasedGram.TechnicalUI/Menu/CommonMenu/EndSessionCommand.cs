using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu;

public class EndSessionCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<EndSessionCommand>();
    public override async Task Execute(Context ctx)
    {
        m_logger.Information($"Ending session");
        ctx.Working = false;
    }

    public override string GetDescription()
    {
        return "Завершить сеанс";
    }
}