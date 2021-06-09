﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
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
        public DiscordSocketClient Client { get; set; }

        public IServerService ServerService { get; set; }

        private readonly ILogger logger;

        public AdminCommands(ILogger<AdminCommands> logger)
        {
            this.logger = logger;
        }

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

        [Command("list_admin_channels")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ListAdminChannels()
        {

            try
            {
                var adminChannel = await AdminChannelService.GetAll(Context.Guild.Id);
                StringBuilder sb = new StringBuilder();

                sb.Append("Following servers are registered on this guild:\n");

                foreach (var ac in adminChannel)
                {
                    var channel = Client.GetChannel(ac.ChannelId) as SocketTextChannel;

                    sb.Append($"{ac.Server.ServerName} - {channel.Name} - {ac.Server.ServerIp}:{ac.Server.ServerPort}\n");
                }

                await ReplyAsync($"{sb}");
            }
            catch(Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
