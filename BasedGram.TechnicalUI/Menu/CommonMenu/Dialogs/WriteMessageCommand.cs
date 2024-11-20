using BasedGram.Common.Core;
using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Dialogs;

public class WriteMessageCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<WriteMessageCommand>();
    private async Task write_msg(Context ctx, Dialog dial)
    {
        Console.Write("Введите сообщение: ");
        var msg = Console.ReadLine();
        if (msg is null || msg.Length == 0)
        {
            m_logger.Error("message is empty!");
            Console.Write("[!] Содержание сообщения не может быть пустым!");
            return;
        }

        await ctx.DialogService.CreateMessage(ctx.User, dial, msg);
    }
    public override async Task Execute(Context ctx)
    {
        var dial_list = await ctx.DialogService.GetUserDialogs(ctx.User);
        if (dial_list.Count == 0)
        {
            Console.WriteLine($"У вас нет диалогов!");
            return;
        }

        if (dial_list.Count == 1)
        {
            var got = await ((dial_list[0].CreatorID == ctx.User.ID)
                    ? ctx.UserService.GetUser(dial_list[0].TargetID)
                    : ctx.UserService.GetUser(dial_list[0].CreatorID));

            Console.WriteLine($"У вас один диалог с: {got.Login}");
            await write_msg(ctx, dial_list[0]);
        }
        else
        {
            Console.WriteLine($"У вас {dial_list.Count} диалогов. Выберите в какой написать");
            for (int i = 0; i < dial_list.Count; ++i)
            {
                var got = await ((dial_list[i].CreatorID == ctx.User.ID)
                                    ? ctx.UserService.GetUser(dial_list[i].TargetID)
                                    : ctx.UserService.GetUser(dial_list[i].CreatorID));

                Console.WriteLine($"[{i + 1}] {got.Login}");
            }
            Console.Write("Введите номер: ");
            if (!int.TryParse(Console.ReadLine(), out int no))
            {
                m_logger.Error("can't parse int!");
                Console.WriteLine("[!] Ошибка");
                return;
            }
            if (!(1 <= no && no <= dial_list.Count))
            {
                m_logger.Error($"{no} not in [1, {dial_list.Count}]");
                Console.WriteLine("[!] Ошибка");
                return;
            }

            await write_msg(ctx, dial_list[no - 1]);
        }
    }

    public override string GetDescription()
    {
        return "Написать сообщение";
    }
}