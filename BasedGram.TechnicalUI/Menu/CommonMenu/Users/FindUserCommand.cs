using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.CommonMenu.Users;

public class FindUserCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<FindUserCommand>();
    public override async Task Execute(Context ctx)
    {
        Console.Write("Введите имя пользователя: ");
        var to_find = Console.ReadLine();
        m_logger.Information($"User inputted name: {to_find}");
        var list = await ((to_find is null) ? ctx.UserService.ListAllUsers() : ctx.UserService.FindUser(to_find));

        if (list.Count == 0)
        {
            Console.WriteLine($"Пользователи не найдены");
            return;
        }
        
        Console.WriteLine($"Найдено {list.Count} подходящих пользователей:");
        for (int i = 0; i < list.Count; ++i)
        {
            Console.WriteLine($"[{i}] {list[i].Login}");
        }
    }

    public override string GetDescription()
    {
        return "Найти пользователя";
    }
}