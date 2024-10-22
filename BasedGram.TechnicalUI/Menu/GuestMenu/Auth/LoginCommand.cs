using BasedGram.Services.AuthService.Exceptions;
using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.GuestMenu.Auth;

public class LoginCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<LoginCommand>();
    public override async Task Execute(Context ctx)
    {
        Console.Write("Введите имя пользователя: ");
        var name = Console.ReadLine();
        if (name is null || name.Length == 0)
        {
            Console.WriteLine("[!] Логин не может быть пустым!");
            return;
        }
        Console.Write("Введите пароль: ");
        var passwd = Console.ReadLine();
        if (passwd is null || passwd.Length == 0)
        {
            Console.WriteLine("[!] Пароль не может быть пустым!");
            return;
        }
        try
        {
            ctx.User = await ctx.AuthService.LoginUser(name, passwd);
            Console.WriteLine("Успех!");

        }
        catch (UserLoginNotFoundException)
        {
            Console.WriteLine("[!] Ошибка! Пользователь не найден!");
        }
        catch(IncorrectPasswordException)
        {
            Console.WriteLine("[!] Ошибка! Некорректные данные для входа");
        }
    }

    public override string GetDescription()
    {
        return "Авторизация";
    }
}