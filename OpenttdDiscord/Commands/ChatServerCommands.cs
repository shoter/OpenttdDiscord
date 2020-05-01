using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenttdDiscord.Admins;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class ChatServerCommands : ModuleBase<SocketCommandContext>
    {
        public const string registerChatServerCommand = "register_chat_server";
        public const string unregisterChatServerCommand = "unregister_chat_server";


        public IServerService ServerService { get; set; }


        public DiscordSocketClient Client { get; set; }

        public IChatChannelServerService ChatChannelServerService { get; set; }

        [Command(registerChatServerCommand)]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RegisterChatServer(string serverName)
        {
            Server server = await ServerService.Get(Context.Guild.Id, serverName);

            if (server == null)
            {
                await ReplyAsync("This server does not exist!");
                return;
            }

            if (await ChatChannelServerService.Exists(Context.Guild.Id, serverName, Context.Channel.Id))
            {
                await ReplyAsync("This server is already registered here!");
                return;
            }

            await ChatChannelServerService.Insert(Context.Guild.Id, serverName, Context.Channel.Id);
            await ReplyAsync("Done!");
        }

        [Command(unregisterChatServerCommand)]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UnregisterChatServer(string serverName)
        {
            Server server = await ServerService.Get(Context.Guild.Id, serverName);

            if (server == null)
            {
                await ReplyAsync("This server does not exist!");
                return;
            }

            if (!await ChatChannelServerService.Exists(Context.Guild.Id, serverName, Context.Channel.Id))
            {
                await ReplyAsync("This server is not registered here!");
                return;
            }

            await ChatChannelServerService.Remove(Context.Guild.Id, serverName, Context.Channel.Id);
            await ReplyAsync("Done!");
        }

        [Command("list_chat_servers")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ListChatServers()
        {
            var servers = (await ChatChannelServerService.GetAll(Context.Guild.Id)).ToList();

            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < servers.Count; ++i)
            {
                var s = servers[i];
                var channel = Client.GetChannel(s.ChannelId) as SocketTextChannel;

                sb.Append($"{s.Server.ServerName} - {channel.Name} - {s.Server.ServerIp}:{s.Server.ServerPort}");
                if (i != servers.Count() - 1)
                    sb.Append("\n");

            }



            await ReplyAsync(sb.ToString());
        }
    }
}
