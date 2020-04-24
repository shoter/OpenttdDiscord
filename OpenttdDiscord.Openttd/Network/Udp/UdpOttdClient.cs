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
            using UdpClient client = new UdpClient();
            var remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);

            await client.SendAsync(sendPacket.Buffer, sendPacket.Size, remoteEP);
            Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
            var receiveBytesTask =  client.ReceiveAsync();

            await Task.WhenAny(timeoutTask, receiveBytesTask);

            if(timeoutTask.IsCompleted)
            {
                throw new OttdConnectionException("Udp server did not respond!");
            }

            return this.udpPacketService.ReadPacket(new Packet(receiveBytesTask.Result.Buffer));
        }
    }
}
