using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class TcpPacketService : ITcpPacketService
    {
        public Packet CreatePacket(ITcpMessage message)
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
                case PacketClientChatMessage chat:
                    {
                        packet.SendByte((byte)(((byte)NetworkAction.NETWORK_ACTION_CHAT) + ((byte)chat.ChatDestination)));
                        packet.SendByte((byte)chat.ChatDestination);
                        packet.SendU32(chat.Destination);
                        packet.SendString(chat.Message);
                        packet.SendU64(chat.Data);

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

        public ITcpMessage ReadPacket(Packet packet)
        {
            var type = (TcpMessageType)packet.ReadByte();

            switch (type)
            {
                case TcpMessageType.PACKET_SERVER_FRAME:
                    {
                        uint frameCounter = packet.ReadU32();
                        uint frameCounterMax = packet.ReadU32();
                        byte token = 0;
                        if (packet.Position < packet.Size)
                            token = packet.ReadByte();

                        return new PacketServerFrameMessage(frameCounter, frameCounterMax, token);
                    }
                case TcpMessageType.PACKET_SERVER_SYNC:
                    {
                        uint frameCounter = packet.ReadU32();
                        uint seed = packet.ReadU32();

                        return new PacketServerSyncMessage(frameCounter, seed);
                    }
                case TcpMessageType.PACKET_SERVER_WELCOME:
                    {
                        uint clientId = packet.ReadU32();
                        uint generationSeed = packet.ReadU32();
                        string networkId = packet.ReadString();

                        return new PacketServerWelcomeMessage(clientId, generationSeed, networkId);
                    }
                case TcpMessageType.PACKET_SERVER_ERROR:
                    {
                        return new PacketServerErrorMessage(errorCode: packet.ReadByte());
                    }
                case TcpMessageType.PACKET_SERVER_ERROR_QUIT:
                    {
                        return new PacketServerErrorQuitMessage(clientId: packet.ReadU32());
                    }
                case TcpMessageType.PACKET_SERVER_QUIT:
                    {
                        return new PacketServerQuitMessage(clientId: packet.ReadU32());
                    }
                case TcpMessageType.PACKET_SERVER_CLIENT_INFO:
                    {
                        uint clientId = packet.ReadU32();
                        byte playas = packet.ReadByte();
                        string clientName = packet.ReadString();

                        return new PacketServerClientInfoMessage(clientId, playas, clientName);
                    }
                case TcpMessageType.PACKET_SERVER_JOIN:
                    {
                        return new PacketServerJoinMessage(packet.ReadU32());
                    }
                case TcpMessageType.PACKET_SERVER_CHAT:
                    {
                        NetworkAction action = (NetworkAction)packet.ReadByte();
                        uint clientId = packet.ReadU32();
                        bool selfSend = packet.ReadBool();
                        string msg = packet.ReadString();
                        long data = packet.ReadI64();

                        return new PacketServerChatMessage(action, clientId, selfSend, msg, data);
                    }
                case TcpMessageType.PACKET_SERVER_FULL:
                case TcpMessageType.PACKET_SERVER_BANNED:
                case TcpMessageType.PACKET_SERVER_MAP_BEGIN:
                case TcpMessageType.PACKET_SERVER_MAP_SIZE:
                case TcpMessageType.PACKET_SERVER_MAP_DATA:
                case TcpMessageType.PACKET_SERVER_MAP_DONE:
                case TcpMessageType.PACKET_SERVER_NEED_GAME_PASSWORD:
                case TcpMessageType.PACKET_SERVER_NEWGAME:
                case TcpMessageType.PACKET_SERVER_SHUTDOWN:
                case TcpMessageType.PACKET_SERVER_CONFIG_UPDATE:
                case TcpMessageType.PACKET_SERVER_COMPANY_UPDATE:
                case TcpMessageType.PACKET_SERVER_MOVE:
                case TcpMessageType.PACKET_SERVER_COMMAND:
                case TcpMessageType.PACKET_SERVER_CHECK_NEWGRFS:

                case TcpMessageType.PACKET_SERVER_RCON:
                    {
                        return new GenericTcpMessage(type);
                    }
                default:
                    {
                        throw new NotImplementedException(type.ToString());
                    }
            }
        }
    }
}
