using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LandOfRails_Website.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LandOfRails_Website.Services
{
    public class CommandHandlingService
    {
        private static DiscordSocketClient client;
        private readonly IServiceProvider _services;
        
        public CommandHandlingService(IServiceProvider services)
        {
            client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            client.Ready += ClientOnReady;
            client.SlashCommandExecuted += SlashCommandHandler;
        }

        public void Register()
        {

        }

        private async Task ClientOnReady()
        {
            var guild = client.GetGuild(394112479283904512);
            var addCommand = new SlashCommandBuilder()
                .WithName("add-download")
                .WithDescription("Adds a download entry to the website")
                .AddOption("title", ApplicationCommandOptionType.String, "Title of the world", true)
                .AddOption("description", ApplicationCommandOptionType.String, "Description of the world", false)
                .AddOption("downloadlink", ApplicationCommandOptionType.String, "DIRECT download link to the world",
                    true)
                .AddOption("backgroundimagelink", ApplicationCommandOptionType.String,
                    "Link to an image of e.g. the world for the background", false)
                .WithDMPermission(false)
                .WithDefaultMemberPermissions(GuildPermission.SendTTSMessages);
            var removeCommand = new SlashCommandBuilder()
                .WithName("remove-download")
                .WithDescription("Removes a download entry to the website")
                .AddOption("title", ApplicationCommandOptionType.String, "Exact title given to the world", true)
                .WithDMPermission(false)
                .WithDefaultMemberPermissions(GuildPermission.SendTTSMessages);

            try
            {
                await guild.CreateApplicationCommandAsync(addCommand.Build());
                await guild.CreateApplicationCommandAsync(removeCommand.Build());
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Data, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.DeferAsync();
            await using var context = new landofrails_websiteContext();
            switch (command.Data.Name)
            {
                case "add-download":
                    var title = command.Data.Options.First(x => x.Name.Equals("title")).Value.ToString();
                    if (context.Downloads.AsEnumerable().Any(x => x.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        await command.ModifyOriginalResponseAsync(properties => properties.Content = "Download with this title already exists. Please choose a different title.");}
                    else
                    {
                        var download = new Download
                        {
                            Title = title,
                            Description = command.Data.Options.FirstOrDefault(x => x.Name.Equals("description"))?.Value.ToString(),
                            DownloadLink = command.Data.Options.First(x => x.Name.Equals("downloadlink")).Value.ToString(),
                            BackgroundImageLink = command.Data.Options.FirstOrDefault(x => x.Name.Equals("backgroundimagelink"))?.Value.ToString(),
                        };
                        context.Downloads.Add(download);
                        await context.SaveChangesAsync();
                        await command.ModifyOriginalResponseAsync(properties => properties.Content = "Download link added. View here: https://www.landofrails.net/downloads");
                    }
                    break;
                case "remove-download":
                    var entry = context.Downloads.AsEnumerable().FirstOrDefault(x => x.Title.Equals(command.Data.Options.FirstOrDefault(x => x.Name.Equals("title")).Value.ToString(), StringComparison.CurrentCultureIgnoreCase));
                    if (entry == null) {
                        await command.ModifyOriginalResponseAsync(properties => properties.Content = "Couldn't find entry. Please ensure you used the exact title or contact MarkenJaden.");
                    }
                    else
                    {
                        context.Downloads.Remove(entry);
                        await context.SaveChangesAsync();
                        await command.ModifyOriginalResponseAsync(properties => properties.Content = "Download removed.");
                    }
                    break;
            }
        }
    }
}
