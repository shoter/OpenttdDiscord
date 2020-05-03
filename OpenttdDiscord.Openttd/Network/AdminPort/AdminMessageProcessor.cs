using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminMessageProcessor : IAdminMessageProcessor
    {
        public IAdminEvent ProcessMessage(IAdminMessage adminMessage, in IAdminPortClient client)
        {
            switch(adminMessage.MessageType)
            {
                case AdminMessageType.ADMIN_PACKET_SERVER_CHAT:
                    {
                        var msg = adminMessage as AdminServerChatMessage;
                        if (msg.NetworkAction != NetworkAction.NETWORK_ACTION_SERVER_MESSAGE)
                            return null;
                        var player = client.Players[msg.ClientId];

                        return new AdminChatMessageEvent(player, msg.Message, client.ServerInfo);
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE:
                    {
                        var msg = adminMessage as AdminServerConsoleMessage;

                        return new AdminConsoleEvent(client.ServerInfo, msg.Origin, msg.Message);
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_RCON:
                    {
                        var msg = adminMessage as AdminServerRconMessage;

                        return new AdminRconEvent(client.ServerInfo, msg.Result);
                    }
                default:
                    return null;
            }
        }
    }
}
