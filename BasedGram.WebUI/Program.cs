using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using BasedGram.WebUI.Utils;

using BasedGram.Services.UserService;
using BasedGram.Services.P2PService;
using BasedGram.Services.AuthService;
using BasedGram.Services.DialogService;

using BasedGram.Database.Core.Repositories;
using BasedGram.Database;
using BasedGram.Database.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore.Storage;
using MongoDB.Driver;
using BasedGram.Common.Core;
using Asp.Versioning;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<JwtProvider>();
            builder.Services.AddSerilog();
            builder.Configuration.AddConfiguration(configuration);

            builder.Services.AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.ReportApiVersions = true;
                })
                .AddMvc() // This is needed for controllers
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });

            builder.Services.AddControllers();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasedGram.Api V1", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "BasedGram.Api V2", Version = "v2" });

                c.EnableAnnotations();
            });

            builder.Services.AddCors();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    // options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        // ValidIssuer = configuration["Jwt:Issuer"],
                        // ValidAudience = configuration["Jwt:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                    };

                    options.Events = new()
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["access-token"];
                            if (context.Token is not null)
                            {
                                var parsed_token = new JwtSecurityToken(context.Token);
                                context.HttpContext.Items["userId"] = parsed_token.Claims.ElementAt(0).Value;
                            }
                            // context.Request.Items["ass"] = 12;

                            return Task.CompletedTask;
                        }
                    };
                });
            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            // builder.Services.AddDbContext<MewingPadDbContext>(opt =>
            // {
            //     opt.UseNpgsql(configuration.GetConnectionString("default"));
            // });
            builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            if (configuration["SelectedDb"]! is "pg")
            {
                builder.Services.AddDbContext<BasedGramNpgsqlDbContext>(opt =>
                {
                    opt.UseNpgsql(configuration.GetConnectionString("postgresql"));
                }, ServiceLifetime.Transient);

                builder.Services.AddScoped<IUserRepository, BasedGram.Database.NpgsqlRepositories.UserRepository>();
                builder.Services.AddScoped<IMessageRepository, BasedGram.Database.NpgsqlRepositories.MessageRepository>();
                builder.Services.AddScoped<IDialogRepository, BasedGram.Database.NpgsqlRepositories.DialogRepository>();
            }
            else if (configuration["SelectedDb"]! is "mongodb")
            {
                var client = new MongoClient(configuration.GetConnectionString("mongodb"));
                builder.Services.AddDbContext<BasedGramMongoDbContext>(opt =>
                {
                    opt.UseMongoDB(client, "BasedGramDB");
                }, ServiceLifetime.Transient);

                builder.Services.AddScoped<IUserRepository, BasedGram.Database.MongoDBRepositories.UserRepository>();
                builder.Services.AddScoped<IMessageRepository, BasedGram.Database.MongoDBRepositories.MessageRepository>();
                builder.Services.AddScoped<IDialogRepository, BasedGram.Database.MongoDBRepositories.DialogRepository>();
            }

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDialogService, DialogService>();

            builder.Services.AddSingleton<IP2PService, P2PService>();

            // builder.Services.AddSingleton<AudioManager>();
            // builder.Services.AddScoped<IUserRepository, UserRepository>();
            // builder.Services.AddScoped<IAudiotrackRepository, AudiotrackRepository>();
            // builder.Services.AddScoped<IPlaylistAudiotrackRepository, PlaylistAudiotrackRepository>();
            // builder.Services.AddScoped<ITagAudiotrackRepository, TagAudiotrackRepository>();
            // builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            // builder.Services.AddScoped<ITagRepository, TagRepository>();
            // builder.Services.AddScoped<IScoreRepository, ScoreRepository>();
            // builder.Services.AddScoped<ICommentaryRepository, CommentaryRepository>();
            // builder.Services.AddScoped<IReportRepository, ReportRepository>();

            // builder.Services.AddScoped<IUserService, UserService>();
            // builder.Services.AddScoped<IOAuthService, OAuthService>();
            // builder.Services.AddScoped<IAudiotrackService, AudiotrackService>();
            // builder.Services.AddScoped<IPlaylistService, PlaylistService>();
            // builder.Services.AddScoped<ITagService, TagService>();
            // builder.Services.AddScoped<IScoreService, ScoreService>();
            // builder.Services.AddScoped<ICommentaryService, CommentaryService>();
            // builder.Services.AddScoped<IReportService, ReportService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasedGram.V1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "BasedGram.V2");
                    // c.RoutePrefix = string.Empty;
                });
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(b => b.WithOrigins(["http://192.168.103.117:3000", "http://192.168.0.102:3000", "http://localhost:3000"])
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials()
                              .Build());



            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
                HttpOnly = HttpOnlyPolicy.None,
                Secure = CookieSecurePolicy.None
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.MapControllers();

            var p2p_serv = app.Services.GetRequiredService<IP2PService>();
            p2p_serv.OnSyncData(async () =>
            {
                Console.WriteLine("Sync data");
                var all_users_list = await app.Services.GetRequiredService<IUserRepository>().ListAllUsers();
                all_users_list.AddRange(await app.Services.GetRequiredService<IUserRepository>().ListAllAdmins());
                var all_dialogs_list = await app.Services.GetRequiredService<IDialogRepository>().ListAllDialogs();
                var all_msgs_list = await app.Services.GetRequiredService<IMessageRepository>().GetAllMessages();

                await p2p_serv.SendUserList(all_users_list);
                await p2p_serv.SendDialogList(all_dialogs_list);
                all_msgs_list.ForEach(async (msg) => { await p2p_serv.SendMessage(msg); });
            });

            p2p_serv.OnUserListReceive(async (List<User> users) =>
            {
                users.ForEach(async (user) =>
                {
                    await app.Services.GetRequiredService<IUserRepository>().UpdateUser(user);
                });
            });

            p2p_serv.OnDialogListReceive(async (List<Dialog> dialogs) =>
            {
                dialogs.ForEach(async (dialog) =>
                {
                    await app.Services.GetRequiredService<IDialogRepository>().Update(dialog);
                });
            });

            p2p_serv.OnMessageReceive(async (Message message) =>
            {
                await app.Services.GetRequiredService<IMessageRepository>().UpdateMessage(message);
            });

            // app.Services.GetRequiredService<IP2PService>().RunNode();


            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

namespace MewingPad
{
    public class Program { }
}
