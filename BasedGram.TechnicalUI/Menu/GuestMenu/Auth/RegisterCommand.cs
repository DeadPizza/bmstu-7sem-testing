using BasedGram.Services.AuthService.Exceptions;
using BasedGram.TechnicalUI.MenuBase;
using Serilog;

namespace BasedGram.TechnicalUI.GuestMenu.Auth;

public class RegisterCommand : Command
{
    private readonly ILogger m_logger = Log.ForContext<RegisterCommand>();
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
        Console.Write("Повторите пароль: ");
        var passwd_repeat = Console.ReadLine();

        if (passwd != passwd_repeat)
        {
            Console.WriteLine("[!] Пароли не совпадают!");
            return;
        }

        try
        {
            await ctx.AuthService.RegisterUser(name, passwd);
            Console.WriteLine("Успех!");
            ctx.User = await ctx.AuthService.LoginUser(name, passwd);

        }
        catch (UserRegisterAlreadyExistsException)
        {
            Console.WriteLine("[!] Ошибка! Пользователь уже существует!");
        }
    }

    public override string GetDescription()
    {
        return "Регистрация";
    }
}