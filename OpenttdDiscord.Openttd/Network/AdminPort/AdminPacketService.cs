using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPacketService : IAdminPacketService
    {
        public Packet CreatePacket(IAdminMessage message)
        {
            Packet packet = new Packet();
            packet.SendByte((byte)message.MessageType);

            switch (message.MessageType)
            {
                case AdminMessageType.ADMIN_PACKET_ADMIN_JOIN:
                    {
                        var msg = message as AdminJoinMessage;
                        packet.SendString(msg.Password, 33);
                        packet.SendString(msg.AdminName, 25);
                        packet.SendString(msg.AdminVersion, 33);
                        break;
                    }

                case AdminMessageType.ADMIN_PACKET_ADMIN_POLL:
                    {
                        var msg = message as AdminPollMessage;
                        packet.SendByte((byte)msg.UpdateType);
                        packet.SendU32(msg.Argument);

                        break;
                    }

                case AdminMessageType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY:
                    {
                        var msg = message as AdminUpdateFrequencyMessage;
                        packet.SendU16((ushort)msg.UpdateType);
                        packet.SendU16((ushort)msg.UpdateFrequency);
                        break;
                    }

                case AdminMessageType.ADMIN_PACKET_ADMIN_CHAT:
                    {
                        var msg = message as AdminChatMessage;
                        packet.SendByte((byte)msg.NetworkAction);
                        packet.SendByte((byte)msg.ChatDestination);
                        packet.SendU32(msg.Destination);
                        packet.SendString(msg.Message, 900);

                        break;
                    }

                case AdminMessageType.ADMIN_PACKET_ADMIN_RCON:
                    {
                        var msg = message as AdminRconMessage;
                        packet.SendString(msg.Command, 500);
                        break;
                    }

                case AdminMessageType.ADMIN_PACKET_ADMIN_PING:
                    {
                        var msg = message as AdminPingMessage;
                        packet.SendU32(msg.Argument);
                        break;
                    }

                case AdminMessageType.ADMIN_PACKET_ADMIN_QUIT:
                case AdminMessageType.ADMIN_PACKET_ADMIN_GAMESCRIPT: //this will be implemented later or never.
                    {
                        break;
                    }

            }

            packet.PrepareToSend();
            return packet;
        }

        public IAdminMessage ReadPacket(Packet packet)
        {
            var type = (AdminMessageType)packet.ReadByte();
            switch (type)
            {
                case AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL:
                    {
                        var dic = new Dictionary<AdminUpdateType, UpdateFrequency>();
                        byte version = packet.ReadByte();

                        bool enabled;
                        while (enabled = packet.ReadBool())
                        {
                            AdminUpdateType updateType = (AdminUpdateType)packet.ReadU16();
                            ushort frequency = packet.ReadU16();
                            dic.Add(updateType, (UpdateFrequency)frequency);
                        }

                        return new AdminServerProtocolMessage(version, dic);
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_WELCOME:
                    {
                        return new AdminServerWelcomeMessage()
                        {
                            ServerName = packet.ReadString(),
                            NetworkRevision = packet.ReadString(),
                            IsDedicated = packet.ReadBool(),
                            MapName = packet.ReadString(),
                            MapSeed = packet.ReadU32(),
                            Landscape = (Landscape)packet.ReadByte(),
                            CurrentDate = new OttdDate(packet.ReadU32()),
                            MapWidth = packet.ReadU16(),
                            MapHeight = packet.ReadU16(),
                        };
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_DATE:
                    {
                        return new AdminServerDateMessage(packet.ReadU32());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN:
                    {
                        return new AdminServerClientJoinMessage(packet.ReadU32());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO:
                    {
                        return new AdminServerClientInfoMessage()
                        {
                            ClientId = packet.ReadU32(),
                            Hostname = packet.ReadString(),
                            ClientName = packet.ReadString(),
                            Language = packet.ReadByte(),
                            JoinDate = new OttdDate(packet.ReadU32()),
                            PlayingAs = packet.ReadByte(),
                        };
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE:
                    {
                        uint clientId = packet.ReadU32();
                        string clientName = packet.ReadString();
                        byte playingAs = packet.ReadByte();
                        return new AdminServerClientUpdateMessage(clientId, clientName, playingAs);
                    }

                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_QUIT:
                    {
                        return new AdminServerClientQuit(packet.ReadU32());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_ERROR:
                    {
                        return new AdminServerClientErrorMessage(packet.ReadU32(), packet.ReadByte());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_NEW:
                    {
                        return new AdminServerCompanyNewMessage(packet.ReadByte());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_INFO:
                    {
                        var m = new AdminServerCompanyInfoMessage();
                        m.CompanyId = packet.ReadByte();
                        m.CompanyName = packet.ReadString();
                        m.ManagerName = packet.ReadString();
                        m.Color = packet.ReadByte();
                        m.HasPassword = packet.ReadBool();
                        m.CreationDate = new OttdDate(packet.ReadU32());
                        m.IsAi = packet.ReadBool();
                        m.MonthsOfBankruptcy = packet.ReadByte();
                        for (int i = 0; i < m.ShareOwnersIds.Length; ++i) m.ShareOwnersIds[i] = packet.ReadByte();

                        return m;
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_UPDATE:
                    {
                        var m = new AdminServerCompanyUpdateMessage();
                        m.CompanyId = packet.ReadByte();
                        m.CompanyName = packet.ReadString();
                        m.ManagerName = packet.ReadString();
                        m.Color = packet.ReadByte();
                        m.HasPassword = packet.ReadBool();
                        m.MonthsOfBankruptcy = packet.ReadByte();
                        for (int i = 0; i < m.ShareOwnersIds.Length; ++i) m.ShareOwnersIds[i] = packet.ReadByte();

                        return m;
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_REMOVE:
                    {
                        return new AdminServerCompanyRemoveMessage(packet.ReadByte(), packet.ReadByte());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_ECONOMY:
                    {
                        var m = new AdminServerCompanyEconomyMessage();
                        m.CompanyId = packet.ReadByte();
                        m.Money = packet.ReadU64();
                        m.CurrentLoan = packet.ReadU64();
                        m.Income = packet.ReadU64();
                        m.DeliveredCargo = packet.ReadU16();

                        // TODO : add quarters.

                        return m;
                    }

                case AdminMessageType.ADMIN_PACKET_SERVER_CHAT:
                    {
                        var m = new AdminServerChatMessage();
                        m.NetworkAction = (NetworkAction)packet.ReadByte();
                        m.ChatDestination = (ChatDestination)packet.ReadByte();
                        m.ClientId = packet.ReadU32();
                        m.Message = packet.ReadString();
                        m.Data = packet.ReadI64();

                        return m;
                    }

                case AdminMessageType.ADMIN_PACKET_SERVER_RCON:
                    {
                        return new AdminServerRconMessage(packet.ReadU16(), packet.ReadString());
                    }

                case AdminMessageType.ADMIN_PACKET_SERVER_CONSOLE:
                    {
                        return new AdminServerConsoleMessage(packet.ReadString(), packet.ReadString());
                    }
                case AdminMessageType.ADMIN_PACKET_SERVER_PONG:
                    {
                        return new AdminServerPongMessage(packet.ReadU32());
                    }

                case AdminMessageType.ADMIN_PACKET_SERVER_NEWGAME:
                case AdminMessageType.ADMIN_PACKET_SERVER_SHUTDOWN:
                case AdminMessageType.ADMIN_PACKET_SERVER_CMD_NAMES:
                case AdminMessageType.ADMIN_PACKET_SERVER_CMD_LOGGING:
                case AdminMessageType.ADMIN_PACKET_SERVER_GAMESCRIPT:
                case AdminMessageType.ADMIN_PACKET_SERVER_RCON_END:
                case AdminMessageType.ADMIN_PACKET_SERVER_COMPANY_STATS: // I do not need this packet for now.
                    {
                        return new GenericAdminMessage(type);
                    }
                default:
                    throw new ArgumentException(type.ToString());
            }
        }
    }
}
