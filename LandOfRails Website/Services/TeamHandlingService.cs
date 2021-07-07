using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Website.Services
{
    public class TeamHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public TeamHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
        }

        public void Register()
        {
            
        }
    }
}
