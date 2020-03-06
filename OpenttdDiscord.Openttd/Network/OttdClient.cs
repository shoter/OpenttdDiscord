using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.Tcp;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network
{
    public class OttdClient : IOttdClient
    {
        private readonly IUdpOttdClient udpClient;
        private readonly ITcpOttdClient tcpClient;
        private readonly IRevisionTranslator revisionTranslator;
        public ServerInfo ServerInfo { get; }

        public event EventHandler<ReceivedChatMessage> ReceivedChatMessage;

        public ConnectionState ConnectionState => tcpClient.ConnectionState;
        internal OttdClient(ServerInfo serverInfo,ITcpOttdClient tcpClient, IUdpOttdClient udpClient, IRevisionTranslator revisionTranslator)
        {
            this.ServerInfo = serverInfo;
            this.udpClient = udpClient;
            this.tcpClient = tcpClient;
            this.revisionTranslator = revisionTranslator;

            this.tcpClient.MessageReceived += TcpClient_MessageReceived;
        }

        private void TcpClient_MessageReceived(object sender, ITcpMessage e)
        {
            switch(e.MessageType)
            {
                case TcpMessageType.PACKET_SERVER_CHAT:
                    {
                        var m = e as PacketServerChatMessage;
                        if (m.ClientId == this.tcpClient.MyClientId)
                            break;
                        ChatDestination destination = (ChatDestination)((int)m.NetworkAction - (int)NetworkAction.NETWORK_ACTION_CHAT);

                        if (new ChatDestination[] { ChatDestination.DESTTYPE_BROADCAST, ChatDestination.DESTTYPE_CLIENT, ChatDestination.DESTTYPE_TEAM }.Contains(destination))
                        {
                            this.ReceivedChatMessage?.Invoke(this, new ReceivedChatMessage(m.Message, tcpClient.Players[m.ClientId], destination));
                            foreach(var g in this.tcpClient.Players)
                            {
                                var p = g.Value;
                                SendChatMessage($"#{p.ClientId} - {p.Name}");
                            }
                        }
                        break;
                    }
            }
        }

        public async Task<PacketUdpServerResponse> AskAboutServerInfo()
        {
            IUdpMessage response = await udpClient.SendMessage(new PacketUdpClientFindServer(), ServerInfo.ServerIp, ServerInfo.ServerPort);

            return response as PacketUdpServerResponse;
        }

        public Task Disconnect() => this.tcpClient.Stop();

        public async Task JoinGame(string username, string password)
        {
            string revision = (await this.AskAboutServerInfo()).ServerRevision;
            uint newgrfRevision = this.revisionTranslator.TranslateToNewGrfRevision(revision).Revision;

            await this.tcpClient.Start(ServerInfo.ServerIp, ServerInfo.ServerPort, username, password, revision, newgrfRevision);
        }

        public Task<object> SendAdminCommand(string command)
        {
            throw new NotImplementedException();
        }

        public Task SendChatMessage(string message) => this.tcpClient.QueueMessage(new PacketClientChatMessage(ChatDestination.DESTTYPE_BROADCAST, 0, message));
    }
}
