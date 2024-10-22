using BasedGram.Common.Core;
using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Dialogs;

public class ListMessagesCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<ListMessagesCommand>();
    private async Task do_PrintMsgs(Context ctx, Dialog dial)
    {
        if (dial.IsBlockedFlag)
        {
            m_logger.Error($"dialog is blocked!");
            Console.WriteLine("[!] Диалог с пользователем заблокирован!");
            return;
        }
        var messages = await ctx.DialogService.GetAllMessages(dial);
        var other = await ((dial.CreatorID == ctx.User.ID)
                    ? ctx.UserService.GetUser(dial.TargetID)
                    : ctx.UserService.GetUser(dial.CreatorID));

        if (messages.Count == 0)
        {
            Console.WriteLine($"Диалог пуст!");
            return;
        }
        Console.WriteLine($"Всего сообщений: {messages.Count}");
        for (int i = 0; i < messages.Count; ++i)
        {
            var got = (messages[i].SenderID == ctx.User.ID) ? ctx.User : other;
            if (messages[i].SenderID != ctx.User.ID)
            {
                messages[i].isReadFlag = true;
                await ctx.DialogService.UpdateMessage(messages[i]);
            }
            var emote = "";
            // Console.WriteLine(messages[i].ReactionState);
            if (messages[i].ReactionState == Reaction.SenderReaction)
            {
                emote += messages[i].SenderID == ctx.User.ID ? "(Вы: огонь)" : "(Собеседник: огонь)";
            }
            else if (messages[i].ReactionState == Reaction.TargetReaction)
            {
                emote += messages[i].SenderID != ctx.User.ID ? "(Вы: огонь)" : "(Собеседник: огонь)";
            }
            else if (messages[i].ReactionState == Reaction.AllReaction)
            {
                emote += "(Вы и собеседник: огонь)";
            }
            Console.WriteLine($"[{i + 1}] [{messages[i].SendTime}] {got.Login}: {messages[i].Content} {emote}");
        }
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
            await do_PrintMsgs(ctx, dial_list[0]);
        }
        else
        {
            Console.WriteLine($"У вас {dial_list.Count} диалогов. Выберите из какого вывести сообщения");
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
                Console.WriteLine("[!] Ошибка");
                return;
            }
            if (!(1 <= no && no <= dial_list.Count))
            {
                Console.WriteLine("[!] Ошибка");
                return;
            }

            do_PrintMsgs(ctx, dial_list[no - 1]);
        }
    }

    public override string GetDescription()
    {
        return "Посмотреть переписку";
    }
}