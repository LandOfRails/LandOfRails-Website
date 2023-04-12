using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LandOfRails_Website.Models;
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

        private static async Task ClientOnReady()
        {
            var guild = client.GetGuild(394112479283904512);
            var addCommand = new SlashCommandBuilder()
                .WithName("add-download")
                .WithDescription("Adds a download entry to the website")
                .AddOption("title", ApplicationCommandOptionType.String, "Title of the world", true)
                .AddOption("description", ApplicationCommandOptionType.String, "Description of the world", false)
                .AddOption("downloadlink", ApplicationCommandOptionType.String, "DIRECT download link to the world", true)
                .AddOption("backgroundimagelink", ApplicationCommandOptionType.String, "Link to an image of e.g. the world for the background", false);
            var removeCommand = new SlashCommandBuilder()
                .WithName("remove-download")
                .WithDescription("Removes a download entry to the website")
                .AddOption("title", ApplicationCommandOptionType.String, "Exact title given to the world", true);

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

        private static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.DeferAsync();
            var context = new landofrails_websiteContext();
            switch (command.Data.Name)
            {
                case "add-download":
                    var title = command.Data.Options.First(x => x.Name.Equals("title")).Value.ToString();
                    if (context.Downloads.Any(x => x.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase)))
                        await command.RespondAsync(
                            "Download with this title already exists. Please choose a different title.",
                            ephemeral: true);
                    else
                    {
                        var download = new Download
                        {
                            Title = title,
                            DownloadLink = command.Data.Options.First(x => x.Name.Equals("downloadlink")).Value.ToString(),
                        };
                        var description = command.Data.Options.First(x => x.Name.Equals("description")).Value;
                        if (description != null) download.Description = description.ToString();
                        var backgroundimagelink = command.Data.Options.First(x => x.Name.Equals("backgroundimagelink")).Value;
                        if (backgroundimagelink != null) download.BackgroundImageLink = backgroundimagelink.ToString();
                        context.Downloads.Add(download);
                        await context.SaveChangesAsync();
                        await command.RespondAsync("Download link added. View here: https://www.landofrails.net/downloads", ephemeral:true);
                    }
                    break;
                case "remove-download":
                    var entry = context.Downloads.First(x => x.Title.Equals(command.Data.Options.FirstOrDefault(x => x.Name.Equals("title")).Value.ToString(), StringComparison.CurrentCultureIgnoreCase));
                    if (entry == null)
                        await command.RespondAsync(
                            "Couldn't find entry. Please ensure you used the exact title or contact MarkenJaden.",
                            ephemeral: true);
                    else
                    {
                        context.Downloads.Remove(entry);
                        await context.SaveChangesAsync();
                        await command.RespondAsync("Download removed.", ephemeral: true);
                    }
                    break;
            }
        }
    }
}
