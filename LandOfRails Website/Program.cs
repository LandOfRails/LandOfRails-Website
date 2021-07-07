using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LandOfRails_Website.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public async Task MainAsync()
        {
            var token = await File.ReadAllLinesAsync("Sensitive-data");
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, token[0]);
            await client.StartAsync();

            services.GetRequiredService<TeamHandlingService>().Register();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<TeamHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
