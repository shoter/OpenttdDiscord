using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Udp
{
    public class UdpOttdClient : IUdpOttdClient
    {
        private readonly IUdpPacketReader packetReader;
        private readonly IUdpPacketCreator packetCreator;

        public UdpOttdClient(IUdpPacketReader packetReader, IUdpPacketCreator packetCreator)
        {
            this.packetCreator = packetCreator;
            this.packetReader = packetReader;
        }

        public async Task<IUdpMessage> SendMessage(IUdpMessage message, string ip, int port)
        {
            var sendPacket = packetCreator.CreatePacket(message);

            using (UdpClient client = new UdpClient())
            {
                var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

                await client.SendAsync(sendPacket.Buffer, sendPacket.Size, remoteEP);
                var receiveBytes = await client.ReceiveAsync();

                return packetReader.ReadPacket(new Packet(receiveBytes.Buffer));
            }
        }





    }
}
