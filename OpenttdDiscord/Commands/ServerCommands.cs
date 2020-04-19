using Discord;
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
    public class ServerCommands : ModuleBase<SocketCommandContext>
    {
        public const string PutServerInfoString = "register_server";

        public ISubscribedServerService SubscribedServerService { get; set; }
        public IChatChannelServerService chatChannelServerService { get; set; }

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
            if (await this.serverService.Exists(ip, port))
            {
                await ReplyAsync("This server is already registered with this bot.");
            }

            await this.serverService.Getsert(ip, port, serverName);

            await ReplyAsync("Server has been registered.");
        }

        [Command("change_password")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RegisterChatServer(string serverName)
        {
            if(this.serverService.IsPasswordRequestInProgress(Context.User.Id))
            {
                var nsp = this.serverService.GetNewPasswordProcess(Context.User.Id);
                await ReplyAsync($"You are in process of registering {nsp.ServerName} - please check DM.");
                return;
            }

            if (!await this.serverService.Exists(serverName))
            {
                await ReplyAsync($"Server with this name does not exist! Please use {PutServerInfoString} in order to register server with this name!");
                return;
            }

            var inReg = new NewServerPassword()
            {
                ServerName = serverName,
                UserId = Context.User.Id
            };

            this.serverService.InformAboutNewPasswordRequest(inReg);
            await ReplyAsync("Check DM to complete process of changing password for this server");
        }
    }
}
