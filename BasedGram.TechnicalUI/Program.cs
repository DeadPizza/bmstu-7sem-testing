using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using BasedGram.TechnicalUI.MenuBase;
using BasedGram.TechnicalUI.MenuBuilders;
using BasedGram.Database.Context;
using BasedGram.Database.Core.Repositories;
using BasedGram.Database.NpgsqlRepositories;
using BasedGram.Services.UserService;
using BasedGram.Services.P2PService;

using Serilog;
using BasedGram.Services.AuthService;
using BasedGram.Services.DialogService;

namespace BasedGram.TechnicalUI;

public static class Program
{
    [STAThread]
    static async Task Main()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        var builder = new HostBuilder().ConfigureServices((hostContext, services) =>
        {
            services.AddDbContext<BasedGramDbContext>(opt =>
            {
                opt.UseNpgsql(config.GetConnectionString("default"));
            }, ServiceLifetime.Transient);

            var menus = new List<Menu>
            {
                new GuestMenuBuilder().Build(),
                new UserMenuBuilder().Build(),
                new AdminMenuBuilder().Build()
            };

            services.AddSingleton(config);
            services.AddSingleton(menus);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IDialogRepository, DialogRepository>();

            services.AddScoped<Context>();

            services.AddScoped<UserService>();
            services.AddScoped<P2PService>();
            services.AddScoped<AuthService>();
            services.AddScoped<DialogService>();

            services.AddSingleton<Startup>();
        });

        var host = builder.Build();

        await using var context = host.Services.GetRequiredService<BasedGramDbContext>();
        await context.Database.MigrateAsync();

        using (var serviceScope = host.Services.CreateAsyncScope())
        {
            var services = serviceScope.ServiceProvider;
            // try
            // {

            Console.WriteLine("Запуск приложения...");
            var techUiApp = services.GetRequiredService<Startup>();
            await techUiApp.Run();
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine("[!] " + ex.Message);
            // }
        }
    }
}