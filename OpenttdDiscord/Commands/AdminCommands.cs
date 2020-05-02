using Discord;
using Discord.Commands;
using OpenttdDiscord.Admins;
using OpenttdDiscord.Database.Admins;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        public IAdminChannelService AdminChannelService { get; set; }
        public IServerService ServerService { get; set; }

        [Command("register_admin_channel")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RegisterChatServer(string serverName, string prefix)
        {
            var server = await ServerService.Get(Context.Guild.Id, serverName);

            if(server == null)
            {
                await ReplyAsync("Server does not exist!");
                return;
            }

            var adminChannel = await AdminChannelService.Get(Context.Channel.Id);

            if(adminChannel != null)
            {
                await ReplyAsync($"{adminChannel.Server.ServerName} is already registered on this channel!");
                return;
            }

            await AdminChannelService.Add(server, Context.Channel.Id, prefix);
            await ReplyAsync("Server has been registered!");
        }

        [Command("change_prefix")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangePrefix(string newPrefix)
        {
            var adminChannel = await AdminChannelService.Get(Context.Channel.Id);

            if (adminChannel == null)
            {
                await ReplyAsync($"{adminChannel.Server.ServerName} is not registered on this channel!");
                return;
            }

            await AdminChannelService.ChangePrefix(adminChannel, newPrefix);
            await ReplyAsync("Prefix has been updated!");
        }

        [Command("unregister_admin_channel")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RegisterChatServer()
        {
            var adminChannel = await AdminChannelService.Get(Context.Channel.Id);

            if (adminChannel == null)
            {
                await ReplyAsync($"{adminChannel.Server.ServerName} is not registered on this channel!");
                return;
            }

            await AdminChannelService.Remove(adminChannel);
            await ReplyAsync("Admin Channel has been removed!");

        }
    }
}
