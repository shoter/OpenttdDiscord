using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Udp
{
    public class UdpPacketCreator : IUdpPacketCreator
    {
        public Packet CreatePacket(IUdpMessage message)
        {
            Packet packet = new Packet();
            packet.SendByte((byte)message.MessageType);
            packet.PrepareToSend();
            return packet;
        }
    }
}
