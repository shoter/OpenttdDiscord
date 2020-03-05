using OpenttdDiscord.Openttd.Network.Tcp;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Network.Openttd
{
    /// <summary>
    /// 1 ottd client per server
    /// </summary>
    public interface IOttdClient
    {
        ConnectionState ConnectionState { get; }

        Task JoinGame(string username, string password);

        /// <summary>
        /// Throws exception if server is stopped.
        /// </summary>
        Task Disconnect();

        Task<PacketUdpServerResponse> AskAboutServerInfo();

        Task SendChatMessage(string message);

        Task<object> SendAdminCommand(string command);
    }
}
