using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Udp
{
    public class UdpPacketReader : IUdpPacketReader
    {
        public IUdpMessage ReadPacket(Packet packet)
        {
            // this variable will be probably not needed :\.
            UdpMessageType type = (UdpMessageType)packet.ReadByte();

            switch(type)
            {
                case UdpMessageType.PACKET_UDP_SERVER_RESPONSE:
                    {
                        PacketUdpServerResponse r = new PacketUdpServerResponse();
                        r.GameVersion = packet.ReadByte();

                        if (r.GameVersion >= 4)
                        {
                            byte newGrfCount = packet.ReadByte();
                            r.ActiveNewGrfs = new PacketUdpServerResponse.ActiveNewGrf[newGrfCount];

                            for (int i = 0; i < newGrfCount; ++i)
                            {
                                PacketUdpServerResponse.ActiveNewGrf grf = new PacketUdpServerResponse.ActiveNewGrf();
                                grf.GrfId = packet.ReadU32();

                                for (int j = 0; j < 16; ++j)
                                {
                                    grf.Md5[j] = packet.ReadByte();
                                }
                            }
                        }

                        r.GameDate = new OttdDate(packet.ReadU32());
                        r.StartDate = new OttdDate(packet.ReadU32());

                        r.CompaniesMax = packet.ReadByte();
                        r.CompaniesOn = packet.ReadByte();
                        r.SpectactorsMax = packet.ReadByte();

                        r.ServerName = packet.ReadString();
                        r.ServerRevision = packet.ReadString();

                        r.LanguageId = packet.ReadByte();
                        r.HasPassword = packet.ReadBool();
                        r.ClientsMax = packet.ReadByte();
                        r.ClientsOn = packet.ReadByte();
                        r.SpectactorsOn = packet.ReadByte();
                        r.MapName = packet.ReadString();
                        r.MapWidth = packet.ReadU16();
                        r.MapHeight = packet.ReadU16();
                        r.MapSet = packet.ReadByte();
                        r.IsDedicated = packet.ReadBool();

                        return r;
                    }
            }

            return null;
        }
    }
}
