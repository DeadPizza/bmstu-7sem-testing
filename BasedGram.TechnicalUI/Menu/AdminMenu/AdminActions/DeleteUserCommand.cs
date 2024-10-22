using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.AdminMenu.AdminActions;

public class DeleteUserCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<DeleteUserCommand>();
    public override async Task Execute(Context ctx)
    {
        Console.Write("Введите имя пользователя: ");
        var to_find = Console.ReadLine();
        m_logger.Information($"name inputted: {to_find}");
        if (to_find is null || to_find.Length == 0)
        {
            m_logger.Error($"username is empty!");
            Console.Write("[!] Имя пользователя не может быть пустым!");
            return;
        }

        var found = await ctx.UserService.FindUser(to_find);
        found.RemoveAll(u => u.Login != to_find);
        if (found.Count != 1)
        {
            Console.WriteLine("Найдено несколько пользователей с такими именами. Выберите конкретного пользователя");
            for (int i = 0; i < found.Count; ++i)
            {
                Console.WriteLine($"[{i + 1}] {found[i].Login}");
            }
            Console.Write("Введите номер: ");
            if (!int.TryParse(Console.ReadLine(), out int no))
            {
                m_logger.Error($"can't parse int!");
                Console.WriteLine("[!] Ошибка");
                return;
            }
            m_logger.Information($"number inputted: {no}");
            if (!(1 <= no && no <= found.Count))
            {
                m_logger.Error($"{no} not in [1, {found.Count}]");
                Console.WriteLine("[!] Ошибка");
                return;
            }

            await ctx.UserService.EraseUser(found[no - 1]);
            Console.WriteLine("Успех!");
        }
        else if (found.Count == 1)
        {
            // await ctx.DialogService.CreateDialog(ctx.User, found[0]);
            await ctx.UserService.EraseUser(found[0]);
            Console.WriteLine("Успех!");
        }
        else
        {
            m_logger.Error($"user not found!");
            Console.WriteLine("[!] Ошибка! Пользователь не найден");
        }
    }

    public override string GetDescription()
    {
        return "Удалить пользователя";
    }
}