using Microsoft.Extensions.Logging;
using NLog;
using OpenttdDiscord.Common;
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
        private readonly ConcurrentQueue<ITcpMessage> sendMessageQueue = new ConcurrentQueue<ITcpMessage>();
        private readonly ConcurrentQueue<ITcpMessage> internalSendMessageQueue = new ConcurrentQueue<ITcpMessage>();
        private readonly ConcurrentQueue<ITcpMessage> receivedMessageQueue = new ConcurrentQueue<ITcpMessage>();
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private readonly ITcpPacketReader packetReader;
        private readonly ITcpPacketCreator packetCreator;
        private readonly IUdpPacketCreator udpPacketCreator;
        private readonly IUdpPacketReader udpPacketReader;
        private readonly bool connected = false;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly ConcurrentDictionary<uint, Player> players = new ConcurrentDictionary<uint, Player>();

        TcpMessageType[] ignoredLoggedTypes = new TcpMessageType[]
{
                TcpMessageType.PACKET_SERVER_SYNC,
                TcpMessageType.PACKET_CLIENT_ACK,
                TcpMessageType.PACKET_SERVER_FRAME
};

        private uint myClientId = 0;
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

        private void QueueInternal(ITcpMessage message)
        {
            this.internalSendMessageQueue.Enqueue(message);
        }

        public async void updateEvents(CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                ITcpMessage message = null;
                try
                {
                    if (receivedMessageQueue.TryDequeue(out message))
                    {
                        if (!ignoredLoggedTypes.Contains(message.MessageType))
                            this.logger.LogTrace($"Received {message.MessageType}");
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
            Task sizeTask = null;
            byte[] sizeBuffer = new byte[2];
            this.QueueInternal(await this.CreateJoinMessage(serverIp, serverPort, username));

            while (token.IsCancellationRequested == false)
            {
#if !DEBUG
                try
                {
#endif

                for (int i = 0; i < 100; ++i)
                {
                    if (this.sendMessageQueue.TryDequeue(out ITcpMessage msg))
                    {
                        Packet packet = this.packetCreator.Create(msg);
                        await client.GetStream().WriteAsync(packet.Buffer, 0, packet.Size);
                        if (!ignoredLoggedTypes.Contains(msg.MessageType))
                            this.logger.LogTrace($"Sent {msg.MessageType.ToString()} - {packet.Size}");
                    }
                    else
                        break;
                }

                while (this.internalSendMessageQueue.TryDequeue(out ITcpMessage msg))
                {
                    Packet packet = this.packetCreator.Create(msg);
                    await client.GetStream().WriteAsync(packet.Buffer, 0, packet.Size);
                    if (!ignoredLoggedTypes.Contains(msg.MessageType))
                        this.logger.LogTrace($"Sent Internal {msg.MessageType.ToString()} - {packet.Size}");

                }

                if (sizeTask == null)
                {
                    sizeTask = client.GetStream().ReadAsync(sizeBuffer, 0, 2);
                }

                //for (int i = 0; i < 100 &&client.GetStream().DataAvailable; ++i)
                if (sizeTask?.IsCompleted ?? false)
                {
                    ushort size = BitConverter.ToUInt16(sizeBuffer, 0);

                    if (size <= 2)
                    {
                        sizeTask = null;
                    }
                    else
                    {

                        byte[] content = new byte[size];
                        content[0] = sizeBuffer[0];
                        content[1] = sizeBuffer[1];

                        await client.GetStream().ReadAsync(content, 2, size - 2);

                        sizeTask = null;

                        var packet = new Packet(content);
                        ITcpMessage msg = this.packetReader.Read(packet);

                        if (!ignoredLoggedTypes.Contains(msg.MessageType))
                            this.logger.LogTrace($"Received {msg.MessageType}");

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
                                    this.QueueInternal(new PacketClientGamePasswordMessage(password));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_FRAME:
                                {
                                    var m = msg as PacketServerFrameMessage;
                                    this.QueueInternal(new PacketClientAckMessage(m.FrameCounter, m.Token));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_CHECK_NEWGRFS:
                                {
                                    // Everything ok here xD
                                    this.QueueInternal(new GenericTcpMessage(TcpMessageType.PACKET_CLIENT_NEWGRFS_CHECKED));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_CLIENT_INFO:
                                {
                                    var m = msg as PacketServerClientInfoMessage;
                                    var player = new Player(m.ClientId)
                                    {
                                        Name = m.ClientName
                                    };
                                    this.logger.LogTrace($"{m.ClientId} - {m.ClientName}");
                                    this.players.AddOrUpdate(m.ClientId, player, (_, __) => player);

                                    if (connected)
                                        this.receivedMessageQueue.Enqueue(msg);

                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_WELCOME:
                                {
                                    var m = msg as PacketServerWelcomeMessage;
                                    this.myClientId = m.ClientId;
                                    this.QueueInternal(new GenericTcpMessage(TcpMessageType.PACKET_CLIENT_GETMAP));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_MAP_DONE:
                                {
                                    this.QueueInternal(new GenericTcpMessage(TcpMessageType.PACKET_CLIENT_MAP_OK));
                                    break;
                                }
                            default:
                                {
                                    if (connected == false)
                                        break;

                                    TcpMessageType[] ignoredMessages =
                                {
                                        TcpMessageType.PACKET_SERVER_MAP_BEGIN, TcpMessageType.PACKET_SERVER_MAP_DATA, TcpMessageType.PACKET_SERVER_MAP_SIZE
                                    };

                                    if (ignoredMessages.Contains(msg.MessageType))
                                        break;

                                    this.receivedMessageQueue.Enqueue(msg);
                                    break;
                                }
                        }
                    }
                }

                await Task.Delay(1);
#if !DEBUG
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, $"{nameof(TcpOttdClient)}:{nameof(mainLoop)}");
                    this.players.Clear();
                    this.internalSendMessageQueue.Clear();
                    this.sendMessageQueue.Clear();
                    this.receivedMessageQueue.Clear();
                    this.myClientId = 0;
                    await Task.Delay(60_000); // wait 60 seconds before reconnecting.
                }
#endif
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
                NewgrfVersion = 1 << 28 | 9 << 24 | 3 << 20 | 1 << 19 | 28004,
            };
        }

        public Task Start(string serverIp, int serverPort, string username, string password = "")
        {
            this.logger.LogInformation($"Connecting to {serverIp}:{serverPort}");
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
