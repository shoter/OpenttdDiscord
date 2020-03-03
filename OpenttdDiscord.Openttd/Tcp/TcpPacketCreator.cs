using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class TcpPacketCreator : ITcpPacketCreator
    {
        public Packet Create(ITcpMessage message)
        {
            Packet packet = new Packet();
            packet.SendByte((byte)message.MessageType);

            switch (message)
            {
                case PacketClientJoinMessage j:
                    {
                        packet.SendString(j.OpenttdRevision);
                        packet.SendU32(j.NewgrfVersion);
                        packet.SendString(j.ClientName);
                        packet.SendByte(j.JoinAs);
                        packet.SendByte(j.Language);

                        break;
                    }
                case PacketClientGamePasswordMessage p:
                    {
                        packet.SendString(p.Password);

                        break;
                    }
                case PacketClientAckMessage ack:
                    {
                        packet.SendU32(ack.FrameCounter);
                        packet.SendByte(ack.Token);

                        break;
                    }

                case GenericTcpMessage _:
                    {
                        // here we have only messages that have only type
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException(message.MessageType.ToString());
                    }
            }

            packet.PrepareToSend();
            return packet;
        }
    }
}
