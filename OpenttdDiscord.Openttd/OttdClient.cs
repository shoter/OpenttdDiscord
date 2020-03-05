using OpenttdDiscord.Openttd.Tcp;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public class OttdClient : IOttdClient
    {
        private readonly IUdpOttdClient udpClient;
        private readonly ITcpOttdClient tcpClient;
        private readonly IRevisionTranslator revisionTranslator;
        private readonly ServerInfo serverInfo;

        public ConnectionState ConnectionState => tcpClient.ConnectionState;
        internal OttdClient(ServerInfo serverInfo,ITcpOttdClient tcpClient, IUdpOttdClient udpClient, IRevisionTranslator revisionTranslator)
        {
            this.serverInfo = serverInfo;
            this.udpClient = udpClient;
            this.tcpClient = tcpClient;
            this.revisionTranslator = revisionTranslator;
        }


        public async Task<PacketUdpServerResponse> AskAboutServerInfo()
        {
            IUdpMessage response = await udpClient.SendMessage(new PacketUdpClientFindServer(), serverInfo.ServerIp, serverInfo.ServerPort);

            return response as PacketUdpServerResponse;
        }

        public Task Disconnect() => this.tcpClient.Stop();

        public async Task JoinGame(string username, string password)
        {
            string revision = (await this.AskAboutServerInfo()).ServerRevision;
            uint newgrfRevision = this.revisionTranslator.TranslateToNewGrfRevision(revision).Revision;

            await this.tcpClient.Start(serverInfo.ServerIp, serverInfo.ServerPort, username, password, revision, newgrfRevision);
        }

        public Task<object> SendAdminCommand(string command)
        {
            throw new NotImplementedException();
        }

        public Task SendChatMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}
