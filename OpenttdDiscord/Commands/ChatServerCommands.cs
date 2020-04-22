﻿using Discord;
using Discord.Commands;
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

        public IServerService ServerService { get; set; }

        public IChatChannelServerService ChatChannelServerService { get; set; }

        [Command(registerChatServerCommand)]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RegisterChatServer(string serverName)
        {
            Server server = await ServerService.Get(serverName);

            if (server == null)
            {
                await ReplyAsync("This server does not exist!");
                return;
            }

            if (await ChatChannelServerService.Exists(serverName, Context.Channel.Id))
            {
                await ReplyAsync("This server is already registered here!");
                return;
            }

            await ChatChannelServerService.Insert(serverName, Context.Channel.Id);
        }
    }
}
