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
                        var player = client.Players[msg.ClientId];

                        return new AdminChatMessageEvent(player, msg.Message, client.ServerInfo);
                    }
                default:
                    return null;
            }
        }
    }
}
