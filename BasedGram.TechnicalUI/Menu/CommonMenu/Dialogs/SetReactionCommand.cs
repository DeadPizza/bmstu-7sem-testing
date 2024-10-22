using BasedGram.Common.Core;
using BasedGram.TechnicalUI.MenuBase;
using Microsoft.VisualBasic;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Dialogs;

public class SetReactionCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<SetReactionCommand>();
    private async Task update_Reaction(Context ctx, Dialog dial)
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
            var emote = "";
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

        Console.Write("Введите номер: ");
        if (!int.TryParse(Console.ReadLine(), out int no))
        {
            m_logger.Error($"can't parse int!");
            Console.WriteLine("[!] Ошибка");
            return;
        }
        if (!(1 <= no && no <= messages.Count))
        {
            m_logger.Error($"{no} not in [1, {messages.Count}]");
            Console.WriteLine("[!] Ошибка");
            return;
        }

        Console.Write("Желаете изменить состояние реакции? (y/n)");
        if (Console.Read() == 'y')
        {
            var msg = messages[no - 1];

            if (msg.ReactionState == Reaction.NoReaction)
            {
                msg.ReactionState = (msg.SenderID == ctx.User.ID) ? Reaction.SenderReaction : Reaction.TargetReaction;
            }

            else if (msg.ReactionState == Reaction.AllReaction)
            {
                msg.ReactionState = (msg.SenderID == ctx.User.ID) ? Reaction.TargetReaction : Reaction.SenderReaction;
            }

            else if (msg.ReactionState == Reaction.SenderReaction)
            {
                msg.ReactionState = (msg.SenderID == ctx.User.ID) ? Reaction.NoReaction : Reaction.AllReaction;
            }

            else if (msg.ReactionState == Reaction.TargetReaction)
            {
                msg.ReactionState = (msg.SenderID == ctx.User.ID) ? Reaction.AllReaction : Reaction.NoReaction;
            }

            await ctx.DialogService.UpdateMessage(msg);
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
            await update_Reaction(ctx, dial_list[0]);
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

            await update_Reaction(ctx, dial_list[no - 1]);
        }
    }

    public override string GetDescription()
    {
        return "Изменить реакцию на сообщение";
    }
}