using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class TcpPacketReader : ITcpPacketReader
    {
        public ITcpMessage Read(Packet packet)
        {
            var type = (TcpMessageType)packet.ReadByte();

            switch (type)
            {
                case TcpMessageType.PACKET_SERVER_FRAME:
                    {
                        uint frameCounter = packet.ReadU32();
                        uint frameCounterMax = packet.ReadU32();

                        return new PacketServerFrameMessage(frameCounter, frameCounterMax);
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

                case TcpMessageType.PACKET_SERVER_CHAT:
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
