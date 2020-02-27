using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Net;
using System.Net.Sockets;

namespace OpenttdDiscord
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient readerClient = new UdpClient();
            var remoteEP = new IPEndPoint(IPAddress.Parse("82.177.95.152"), 3980);

            var creator = new UdpPacketCreator();
            var packet = creator.CreatePacket(new PacketUdpClientFindServer());

            readerClient.Send(packet.Buffer, packet.Size, remoteEP);

            byte[] bytes = readerClient.Receive(ref remoteEP);
            Packet received = new Packet(bytes);    

            var reader = new UdpPacketReader();
            var receveived = reader.ReadPacket(received);
            int a = 123;


        }
    }
}
