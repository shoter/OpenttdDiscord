using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Udp
{
    public class UdpOttdClient : IUdpOttdClient
    {
        private readonly IUdpPacketService udpPacketService;

        public UdpOttdClient(IUdpPacketService udpPacketService)
        {
            this.udpPacketService = udpPacketService;
        }

        public async Task<IUdpMessage> SendMessage(IUdpMessage message, string ip, int port)
        {
            var sendPacket = this.udpPacketService.CreatePacket(message);

            using (UdpClient client = new UdpClient())
            {
                var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

                await client.SendAsync(sendPacket.Buffer, sendPacket.Size, remoteEP);
                var receiveBytes = await client.ReceiveAsync();

                return this.udpPacketService.ReadPacket(new Packet(receiveBytes.Buffer));
            }
        }
    }
}
