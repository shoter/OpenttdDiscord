using Microsoft.Extensions.Logging;
using NLog;
using OpenttdDiscord.Openttd.Udp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tcp
{
    public class TcpOttdClient : ITcpOttdClient
    {
        public event EventHandler<ITcpMessage> MessageReceived;
        private ConcurrentQueue<ITcpMessage> sendMessageQueue;
        private ConcurrentQueue<ITcpMessage> internalSendMessageQueue;
        private ConcurrentQueue<ITcpMessage> receivedMessageQueue;
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private readonly ITcpPacketReader packetReader;
        private readonly ITcpPacketCreator packetCreator;
        private readonly IUdpPacketCreator udpPacketCreator;
        private readonly IUdpPacketReader udpPacketReader;
        private readonly bool connected = false;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public TcpOttdClient(ITcpPacketCreator packetCreator, ITcpPacketReader packetReader, IUdpPacketReader udpPacketReader, IUdpPacketCreator udpPacketCreator, ILogger<ITcpOttdClient> logger)
        {
            this.logger = logger;
            this.packetCreator = packetCreator;
            this.packetReader = packetReader;
            this.udpPacketCreator = udpPacketCreator;
            this.udpPacketReader = udpPacketReader;
        }

        public Task QueueMessage(ITcpMessage message)
        {
            this.sendMessageQueue.Enqueue(message);
            return Task.CompletedTask;
        }

        private void QueueInternal(ITcpMessage message) => this.internalSendMessageQueue.Enqueue(message);

        public async void updateEvents(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                ITcpMessage message = null;
                try
                {
                    if (receivedMessageQueue.TryDequeue(out message))
                    {
                        this.MessageReceived?.Invoke(this, message);
                    }

                    if (receivedMessageQueue.IsEmpty)
                        await Task.Delay(1000);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, $"Error with {message?.MessageType}");
                }
            }
        }

        public async void mainLoop(CancellationToken token, string serverIp, int serverPort, string username, string password)
        {
            TcpClient client = new TcpClient(serverIp, serverPort);
            while (token.IsCancellationRequested == false)
            {
                try
                {

                    for (int i = 0; i < 100; ++i)
                    {
                        if (this.sendMessageQueue.TryDequeue(out ITcpMessage msg))
                        {
                            Packet packet = this.packetCreator.Create(msg);
                            await client.GetStream().WriteAsync(packet.Buffer, 0, packet.Size);
                        }
                        else
                            break;
                    }

                    while (this.internalSendMessageQueue.TryDequeue(out ITcpMessage msg))
                    {
                        Packet packet = this.packetCreator.Create(msg);
                        await client.GetStream().WriteAsync(packet.Buffer, 0, packet.Size);
                    }

                    for (int i = 0; i < 100 && client.GetStream().DataAvailable && this.connected; ++i)
                    {
                        NetworkStream s = client.GetStream();

                        // read 2 bytes to get size
                        byte[] size = new byte[2];
                        await s.ReadAsync(size);

                        ushort packetSize = BitConverter.ToUInt16(size, 0);

                        byte[] content = new byte[packetSize];
                        content[0] = size[0];
                        content[1] = size[1];

                        await s.ReadAsync(content, 2, packetSize - 2);

                        var packet = new Packet(content);
                        ITcpMessage msg = this.packetReader.Read(packet);

                        switch (msg.MessageType)
                        {
                            case TcpMessageType.PACKET_SERVER_FULL:
                                {
                                    this.logger.LogWarning($"Cannot join {serverIp}:{serverPort} - server is full");
                                    await Task.Delay(TimeSpan.FromMinutes(1));
                                    this.QueueInternal(await this.CreateJoinMessage(serverIp, serverPort, username));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_BANNED:
                                {
                                    this.logger.LogWarning($"Cannot join {serverIp}:{serverPort} - user is banned");
                                    await Task.Delay(TimeSpan.FromMinutes(10));
                                    this.QueueInternal(await this.CreateJoinMessage(serverIp, serverPort, username));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_NEED_GAME_PASSWORD:
                                {

                                }
                            default:
                                {
                                    this.receivedMessageQueue.Enqueue(msg);
                                    break;
                                }
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, $"{nameof(TcpOttdClient)}:{nameof(mainLoop)}");
                    await Task.Delay(60_000); // wait 60 seconds before reconnecting.
                }
            }

        }

        private async Task<PacketClientJoinMessage> CreateJoinMessage(string serverIp, int serverPort, string username)
        {
            UdpOttdClient udpClient = new UdpOttdClient(this.udpPacketReader, this.udpPacketCreator);
            var response = await udpClient.SendMessage(new PacketUdpClientFindServer(), serverIp, serverPort);
            string revision = (response as PacketUdpServerResponse).ServerRevision;

            return new PacketClientJoinMessage()
            {
                ClientName = username,
                JoinAs = 0,
                Language = 0,
                OpenttdRevision = revision,
                NewgrfVersion = 1 << 28 | 11 << 24 | 0 << 20 | 0 << 19 | 28004,
            };
        }

        public Task Start(string serverIp, int serverPort, string username, string password = "")
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => mainLoop(cancellationTokenSource.Token, serverIp, serverPort, username, password)), null);
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => updateEvents(cancellationTokenSource.Token)), null);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            this.cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
