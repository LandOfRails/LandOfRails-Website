using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LandOfRails_Website.Models;
using LandOfRails_Website.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LandOfRails_Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((hostContext, services) =>
                    {
                        var configuration = hostContext.Configuration;
                        var connectionString = configuration.GetConnectionString("lor_websiteConnection");

                        services.AddDbContextFactory<landofrails_websiteContext>(options =>
                            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                    });
                    webBuilder.UseStartup<Startup>();
                });

        public async Task MainAsync(string[] args)
        {
            await using var services = CnfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var host = CreateHostBuilder(args).Build();
            var hostServices = host.Services;
            var configuration = hostServices.GetRequiredService<IConfiguration>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, configuration["DiscordBot:Token"]);
            await client.StartAsync();

            services.GetRequiredService<TeamHandlingService>().Register();
            services.GetRequiredService<CommandHandlingService>().Register();

            await CreateHostBuilder(args).Build().RunAsync();
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider CnfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<TeamHandlingService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
