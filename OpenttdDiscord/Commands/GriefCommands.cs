using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using OpenttdDiscord.Database.AntiGrief;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Commands
{
    public class GriefCommands : ModuleBase<SocketCommandContext>
    {
        public ITrustedIpService TrustedIpService { get; set; }

        public IAntiGriefService AntiGriefService { get; set; }

        public IServerService serverService { get; set; }


        [Command("add_antigrief")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PutAntiGriefServer(string serverName, int minutesTime, string reason)
        {
            if (!await this.serverService.Exists(Context.Guild.Id, serverName))
            {
                await ReplyAsync("This server does not exist!");
            }

            var server = await this.serverService.Get(Context.Guild.Id, serverName);

            if( await this.AntiGriefService.Exists(server.Id))
            {
                await ReplyAsync("This server is already registered in anti grief system!");
            }

            await this.AntiGriefService.Add(server, TimeSpan.FromMinutes(minutesTime), reason);

            await ReplyAsync("Server added!");
        }

        [Command("change_antigrief")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeAntiGriefServer(string serverName, int minutesTime, string reason)
        {
            if (!await this.serverService.Exists(Context.Guild.Id, serverName))
            {
                await ReplyAsync("This server does not exist!");
            }

            var server = await this.serverService.Get(Context.Guild.Id, serverName);

            if (! await this.AntiGriefService.Exists(server.Id))
            {
                await ReplyAsync("This server is not registered in anti grief system!");
            }

            var agServer = await this.AntiGriefService.Get(server.Id);
            await this.AntiGriefService.Remove(agServer);
            await this.AntiGriefService.Add(server, TimeSpan.FromMinutes(minutesTime), reason);

            await ReplyAsync("Server changed!");
        }

        [Command("remove_antigrief")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RemoveAntiGriefServer(string serverName)
        {
            if (!await this.serverService.Exists(Context.Guild.Id, serverName))
            {
                await ReplyAsync("This server does not exist!");
            }

            var server = await this.serverService.Get(Context.Guild.Id, serverName);

            if (await this.AntiGriefService.Exists(server.Id))
            {
                await ReplyAsync("This server is already registered in anti grief system!");
            }

            var agServer = await this.AntiGriefService.Get(server.Id);
            await this.AntiGriefService.Remove(agServer);

            await ReplyAsync("Server removed from AntiGrief system!");
        }
    }
}
