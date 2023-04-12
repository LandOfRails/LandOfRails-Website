using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LandOfRails_Website.Models;
using LandOfRails_Website.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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
                    webBuilder.ConfigureServices(collection =>
                    {
                        var serverVersion = new MariaDbServerVersion(new Version(10, 6, 12));
                        collection.AddDbContextFactory<landofrails_websiteContext>(options =>
                            options.UseMySql($"server=landofrails.net;uid=landofrails_website;pwd={File.ReadAllLines("Sensitive-data")[1]};database=landofrails_website", serverVersion));
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5005");
                });

        public async Task MainAsync(string[] args)
        {
            var token = await File.ReadAllLinesAsync("Sensitive-data");
            await using var services = CnfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, token[0]);
            await client.StartAsync();

            services.GetRequiredService<TeamHandlingService>().Register();

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
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
