﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenttdDiscord.Common.Networking;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class ServerCommands : ModuleBase<SocketCommandContext>
    {
        public const string PutServerInfoString = "register_server";

        public ISubscribedServerService SubscribedServerService { get; set; }
        public IChatChannelServerService chatChannelServerService { get; set; }

        public DiscordSocketClient client { get; set; }

        public IServerService serverService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command(PutServerInfoString)]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PutServerInfo(string ip, int port, string serverName)
        {
            if (await this.serverService.Exists(Context.Guild.Id, ip, port))
            {
                await ReplyAsync("This server is already registered with this bot.");
            }

            if(!System.Net.IPAddress.TryParse(ip, out _))
            {
                await ReplyAsync("Wrong IP address!");
                return;
            }

            if(NetworkPort.IsCorrect(port) == false)
            {
                await ReplyAsync("Wrong port!");
                return;
            }

            await this.serverService.Getsert(Context.Guild.Id, ip, port, serverName);

            await ReplyAsync("Server has been registered.");
        }

        [Command("change_password")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangePassword(string serverName)
        {
            if(this.serverService.IsPasswordRequestInProgress(Context.User.Id))
            {
                var nsp = this.serverService.GetNewPasswordProcess(Context.User.Id);
                await ReplyAsync($"You are in process of registering {nsp.ServerName} - please check DM.");
                return;
            }

            if (!await this.serverService.Exists(Context.Guild.Id, serverName))
            {
                await ReplyAsync($"Server with this name does not exist! Please use {PutServerInfoString} in order to register server with this name!");
                return;
            }

            var inReg = new NewServerPassword()
            {
                ServerName = serverName,
                UserId = Context.User.Id,
                GuildId = Context.Guild.Id,
            };

            this.serverService.InformAboutNewPasswordRequest(inReg);
            await ReplyAsync("Check DM to complete process of changing password for this server");
        }

        [Command("subscribe_server")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SubscribeToServer(string serverName, int port)
        {
            var server = await serverService.Get(Context.Guild.Id, serverName);

            if(server == null)
            {
                await ReplyAsync("Server does not exist!");
                return;
            }

            if(await SubscribedServerService.Exists(Context.Guild.Id, server.ServerName, Context.Channel.Id))
            {
                await ReplyAsync("Server is already registered here!");
                return;
            }

            if (NetworkPort.IsCorrect(port) == false)
            {
                await ReplyAsync("Wrong port!");
                return;
            }

            await SubscribedServerService.AddServer(Context.Guild.Id, server.ServerName, port, Context.Channel.Id);
            await ReplyAsync("Done!");
            return;
        }

        [Command("unsubscribe_server")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SubscribeToServer(string serverName)
        {
            var server = await serverService.Get(Context.Guild.Id, serverName);

            if (server == null)
            {
                await ReplyAsync("Server does not exist!");
                return;
            }

            if (!await SubscribedServerService.Exists(Context.Guild.Id, server.ServerName, Context.Channel.Id))
            {
                await ReplyAsync("Server is not registered here!");
                return;
            }

            await SubscribedServerService.RemoveServer(Context.Guild.Id, server.ServerName, Context.Channel.Id);
            await ReplyAsync("Done!");
            return;
        }

        [Command("list_subscribed_servers")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ListSubscribedServers()
        {
            var servers = await SubscribedServerService.GetAllServers(Context.Guild.Id);

            StringBuilder sb = new StringBuilder();

            for(int i = 0;i < servers.Count();++i)
            {
                var s = servers.ElementAt(i);
                var channel = client.GetChannel(s.ChannelId) as SocketTextChannel;

                sb.Append($"{s.Server.ServerName} - {channel.Name} - {s.Server.ServerIp}:{s.Port}");
                if (i != servers.Count() - 1)
                    sb.Append("\n");
            }

            await ReplyAsync(sb.ToString());
        }
    }
}
