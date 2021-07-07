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
        private static DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        private readonly ulong Owner = 394112698511654912;

        private readonly ulong ManagementIR = 526513230223376424;
        private readonly ulong ManagementTC = 554030398267457635;
        private readonly ulong ManagementZND = 709849764803510354;
        private readonly ulong ManagementRTM = 554030406656065579;

        private readonly ulong DeputyManagementIR = 532250985293414402;

        private static readonly ulong TeamTC = 438074536508784640;
        private static readonly ulong TeamIR = 456916096587530241;
        private static readonly ulong TeamZND = 709848394725851211;
        private static readonly ulong TeamRTM = 529727596942983187;

        public TeamHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
        }

        public void Register()
        {
            
        }

        public static List<SocketGuildUser> GetTeamMembers()
        {
            List<SocketGuildUser> guildUsers = new List<SocketGuildUser>();

            guildUsers.AddRange(_discord.GetGuild(394112479283904512).GetRole(TeamTC).Members);
            guildUsers.AddRange(_discord.GetGuild(394112479283904512).GetRole(TeamIR).Members);
            guildUsers.AddRange(_discord.GetGuild(394112479283904512).GetRole(TeamZND).Members);
            guildUsers.AddRange(_discord.GetGuild(394112479283904512).GetRole(TeamRTM).Members);

            guildUsers = guildUsers.Distinct().ToList();

            return guildUsers;
        }
    }
}
