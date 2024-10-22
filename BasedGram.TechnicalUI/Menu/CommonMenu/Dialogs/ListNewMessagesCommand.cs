using BasedGram.Common.Core;
using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Dialogs;

public class ListNewMessagesCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<ListNewMessagesCommand>();
    public override async Task Execute(Context ctx)
    {
        var dial_list = await ctx.DialogService.GetUserDialogs(ctx.User);
        int printed = 0;
        foreach(var dial in dial_list)
        {
            if(dial.IsBlockedFlag)
            {
                continue;
            }
            var messages = await ctx.DialogService.GetAllMessages(dial);
            messages.RemoveAll(m => m.SenderID == ctx.User.ID || m.isReadFlag);
            foreach(var msg in messages)
            {
                var sender = await ctx.UserService.GetUser(msg.SenderID);
                Console.WriteLine($"Новое сообщение от {sender.Login}: {msg.Content}");
                ++printed;
            }
        }
        if(printed == 0)
        {
            Console.WriteLine("У вас нет новых сообщений!");
        }
    }

    public override string GetDescription()
    {
        return "Новые сообщения";
    }
}