using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Users;

public class NewDialogCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<NewDialogCommand>();
    public override async Task Execute(Context ctx)
    {
        Console.Write("Введите имя пользователя: ");
        var to_find = Console.ReadLine();
        m_logger.Information($"User inputted name: {to_find}");
        if (to_find is null || to_find.Length == 0)
        {
            m_logger.Error($"input is empty");
            Console.Write("[!] Имя пользователя не может быть пустым!");
            return;
        }

        var found = await ctx.UserService.FindUser(to_find);
        found.RemoveAll(u => u.Login != to_find);
        if (found.Count > 1)
        {
            Console.WriteLine($"Найдено несколько пользователей с такими именами ({found.Count}). Выберите конкретного пользователя");
            for (int i = 0; i < found.Count; ++i)
            {
                Console.WriteLine($"[{i + 1}] {found[i].Login}");
            }
            Console.Write("Введите номер: ");
            if (!int.TryParse(Console.ReadLine(), out int no))
            {
                m_logger.Error("can't parse int!");
                Console.WriteLine("[!] Ошибка");
                return;
            }
            if (!(1 <= no && no <= found.Count))
            {
                m_logger.Error($"{no} not in [1, {found.Count}]");
                Console.WriteLine("[!] Ошибка");
                return;
            }

            await ctx.DialogService.CreateDialog(ctx.User, found[no - 1]);
            Console.WriteLine("Успех!");
        }
        else if (found.Count == 1)
        {
            await ctx.DialogService.CreateDialog(ctx.User, found[0]);
            Console.WriteLine("Успех!");
        }
        else
        {
            Console.WriteLine("[!] Ошибка! Пользователь не найден");
        }
    }

    public override string GetDescription()
    {
        return "Начать диалог";
    }
}