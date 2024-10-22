using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Dialogs;

public class ListDialogsCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<ListDialogsCommand>();
    public override async Task Execute(Context ctx)
    {
        var dial_list = await ctx.DialogService.GetUserDialogs(ctx.User);
        if (dial_list.Count == 0)
        {
            Console.WriteLine($"У вас нет диалогов!");
            return;
        }

        Console.WriteLine($"У вас {dial_list.Count} диалогов");
        for (int i = 0; i < dial_list.Count; ++i)
        {
            var got = await ((dial_list[i].CreatorID == ctx.User.ID)
                                ? ctx.UserService.GetUser(dial_list[i].TargetID)
                                : ctx.UserService.GetUser(dial_list[i].CreatorID));

            Console.WriteLine($"[{i + 1}] {got.Login}");
        }
    }

    public override string GetDescription()
    {
        return "Список всех диалогов";
    }
}