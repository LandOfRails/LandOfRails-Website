using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
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

        public static readonly ulong TeamTC = 438074536508784640;
        public static readonly ulong TeamIR = 456916096587530241;
        public static readonly ulong TeamZND = 709848394725851211;
        public static readonly ulong TeamRTM = 529727596942983187;

        public TeamHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
        }

        public void Register()
        {

        }

        public static List<List<SocketGuildUser>> GetTeamMembers(IEnumerable<ulong> teams)
        {
            List<List<SocketGuildUser>> teamMembers = new List<List<SocketGuildUser>>();

            List<SocketGuildUser> guildUsers = new List<SocketGuildUser>();

            foreach (ulong team in teams) guildUsers.AddRange(_discord.GetGuild(394112479283904512).GetRole(team).Members);

            guildUsers = guildUsers.Distinct().ToList();

            List<SocketGuildUser> removeUsers = guildUsers.Where(user => user.Roles.Any(x => x.Id is 530846961012703252 or 417765243415035914)).Where(user => user.Roles.All(x => x.Id != 554029864747794573)).ToList();

            foreach (SocketGuildUser user in removeUsers) guildUsers.Remove(user);

            guildUsers = guildUsers.OrderByDescending(x => x.Status is UserStatus.Online or UserStatus.DoNotDisturb or UserStatus.AFK or UserStatus.Idle).ToList();

            teamMembers.Add(guildUsers.FindAll(x => x.Status is UserStatus.Online));
            teamMembers.Add(guildUsers.FindAll(x => x.Status is UserStatus.Idle or UserStatus.AFK));
            teamMembers.Add(guildUsers.FindAll(x => x.Status is UserStatus.DoNotDisturb));
            teamMembers.Add(guildUsers.FindAll(x => x.Status is UserStatus.Invisible or UserStatus.Offline));

            teamMembers = teamMembers.Select(list => list.OrderByDescending(x => x.Roles.OrderByDescending(x => x.Position).First()).ToList()).ToList();

            return teamMembers;
        }
    }
}
