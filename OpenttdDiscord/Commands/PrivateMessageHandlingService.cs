using Discord.WebSocket;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class PrivateMessageHandlingService : IPrivateMessageHandlingService
    {
        private readonly IServerService serverService;
        private readonly IAdminPortClientFactory adminPortClientFactory;

        public PrivateMessageHandlingService(IServerService serverService, IAdminPortClientFactory adminPortClientFactory)
        {
            this.serverService = serverService;
            this.adminPortClientFactory = adminPortClientFactory;
        }


        public async Task HandleMessage(SocketUserMessage message, SocketDMChannel channel)
        {
            if(serverService.IsPasswordRequestInProgress(message.Author.Id))
            {
                var nsp = serverService.RemoveNewPasswordRequest(message.Author.Id);
                var server = await serverService.Get(nsp.ServerName);
                if(server == null)
                {
                    await channel.SendMessageAsync("Server was removed in the meantime - request invalid");
                    return;
                }
                IAdminPortClient client = adminPortClientFactory.Create(new ServerInfo(server.ServerIp, server.ServerPort, message.Content));

                try
                {
                    await client.Join();
                }
                catch
                {
                    await channel.SendMessageAsync("Password was incorrect or connection to server was impossible");
                    return;
                }
                finally
                {
                    await client.Disconnect();
                }

                await this.serverService.ChangePassword(server.Id, message.Content);
                await channel.SendMessageAsync("Password has been changed.");
            }
        }
    }
}
