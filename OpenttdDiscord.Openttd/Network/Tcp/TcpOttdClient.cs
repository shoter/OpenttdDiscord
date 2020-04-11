using Microsoft.Extensions.Logging;
using NLog;
using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd.Network;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.Tcp
{
    public class TcpOttdClient : ITcpOttdClient
    {
        public event EventHandler<ITcpMessage> MessageReceived;
        private readonly ConcurrentQueue<ITcpMessage> sendMessageQueue = new ConcurrentQueue<ITcpMessage>();
        private readonly ConcurrentQueue<ITcpMessage> receivedMessageQueue = new ConcurrentQueue<ITcpMessage>();
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private readonly ITcpPacketService tcpPacketService;
        private readonly IRevisionTranslator revisionTranslator;
        private bool connected = false;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public ConnectionState ConnectionState { get; private set; } = ConnectionState.Idle;

        private readonly ConcurrentDictionary<uint, Player> players = new ConcurrentDictionary<uint, Player>();

        private TcpClient client = new TcpClient();

        public uint MyClientId { get; private set; } = 0;

        public IReadOnlyDictionary<uint, Player> Players => this.players.ToImmutableDictionary();

        public TcpOttdClient(ITcpPacketService tcpPacketService, IRevisionTranslator revisionTranslator, ILogger<ITcpOttdClient> logger)
        {
            this.logger = logger;
            this.revisionTranslator = revisionTranslator;
            this.tcpPacketService = tcpPacketService;
        }

        public Task QueueMessage(ITcpMessage message)
        {
            this.sendMessageQueue.Enqueue(message);
            return Task.CompletedTask;
        }
        public async void UpdateEvents(CancellationToken token)
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
                        await Task.Delay(TimeSpan.FromSeconds(0.5));
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, $"Error with {message?.MessageType}");
                }
            }
        }

        public async void MainLoop(CancellationToken token, string serverIp, int serverPort, string username, string password, string revision, uint newgrfRevision)
        {
            Task<int> sizeTask = null;
            byte[] sizeBuffer = new byte[2];
            int sizeBufferPos = 0;


            while (token.IsCancellationRequested == false)
            {
                try
                {
                    if (this.ConnectionState == ConnectionState.NotConnected)
                    {
                        sizeTask = null;
                        this.Reset();

                        this.ConnectionState = ConnectionState.Connecting;
                        client.Connect(serverIp, serverPort);
                        await this.QueueMessage(new PacketClientJoinMessage()
                        {
                            ClientName = username,
                            JoinAs = 255,
                            Language = 0,
                            OpenttdRevision = revision,
                            NewgrfVersion = newgrfRevision
                        });
                    }
                    for (int i = 0; i < 100; ++i)
                    {
                        if (this.sendMessageQueue.TryDequeue(out ITcpMessage msg))
                        {
                            Packet packet = this.tcpPacketService.CreatePacket(msg);
                            await client.GetStream().WriteAsync(packet.Buffer, 0, packet.Size);
                        }
                        else
                            break;
                    }


                    while ((sizeTask ??= client.GetStream().ReadAsync(sizeBuffer, sizeBufferPos, 2 - sizeBufferPos)).IsCompleted)
                    {
                        sizeBufferPos += sizeTask.Result;

                        if (sizeBufferPos == 1)
                        {
                            sizeTask = client.GetStream().ReadAsync(sizeBuffer, sizeBufferPos, 2 - sizeBufferPos);
                            await sizeTask;
                        }

                        sizeBufferPos = 0;
                        sizeTask = null;
                        


                        sizeTask = null;

                        ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
                        byte[] content = new byte[size];
                        content[0] = sizeBuffer[0];
                        content[1] = sizeBuffer[1];
                        int contentSize = 2;


                        do
                        {
                            Task<int> task = client.GetStream().ReadAsync(content, contentSize, size - contentSize);
                            await task;
                            contentSize += task.Result;
                        } while (contentSize < size);

                        var packet = new Packet(content);
                        ITcpMessage msg = this.tcpPacketService.ReadPacket(packet);

                        switch (msg.MessageType)
                        {
                            case TcpMessageType.PACKET_SERVER_FULL:
                                {
                                    this.logger.LogWarning($"Cannot join {serverIp}:{serverPort} - server is full");
                                    await Task.Delay(TimeSpan.FromMinutes(1));
                                    this.Reset();
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_BANNED:
                                {
                                    this.logger.LogWarning($"Cannot join {serverIp}:{serverPort} - user is banned");
                                    await Task.Delay(TimeSpan.FromMinutes(10));
                                    this.Reset();
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_NEED_GAME_PASSWORD:
                                {
                                    await this.QueueMessage(new PacketClientGamePasswordMessage(password));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_FRAME:
                                {
                                    var m = msg as PacketServerFrameMessage;
                                    await this.QueueMessage(new PacketClientAckMessage(m.FrameCounter, m.Token));
                                    this.ConnectionState = ConnectionState.Connected;
                                    this.connected = true;
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_CHECK_NEWGRFS:
                                {
                                    await this.QueueMessage(new GenericTcpMessage(TcpMessageType.PACKET_CLIENT_NEWGRFS_CHECKED));
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_CLIENT_INFO:
                                {
                                    var m = msg as PacketServerClientInfoMessage;
                                    var player = new Player(m.ClientId, m.ClientName);
                                    this.players.AddOrUpdate(m.ClientId, player, (_, __) => player);

                                    if (connected)
                                        this.receivedMessageQueue.Enqueue(msg);

                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_QUIT:
                                {
                                    var m = msg as PacketServerQuitMessage;

                                    this.players.TryRemove(m.ClientId, out _);

                                    this.receivedMessageQueue.Enqueue(msg);
                                    break;

                                }
                            case TcpMessageType.PACKET_SERVER_ERROR_QUIT:
                                {
                                    var m = msg as PacketServerErrorQuitMessage;

                                    this.players.TryRemove(m.ClientId, out _);

                                    this.receivedMessageQueue.Enqueue(msg);
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_WELCOME:
                                {
                                    var m = msg as PacketServerWelcomeMessage;
                                    this.MyClientId = m.ClientId;
                                    await this.QueueMessage(new GenericTcpMessage(TcpMessageType.PACKET_CLIENT_GETMAP));
                                    this.ConnectionState = ConnectionState.DownloadingMap;
                                    break;
                                }
                            case TcpMessageType.PACKET_SERVER_MAP_DONE:
                                {
                                    await this.QueueMessage(new GenericTcpMessage(TcpMessageType.PACKET_CLIENT_MAP_OK));
                                    break;
                                }
                            default:
                                {
                                    if (connected == false)
                                        break;

                                    this.receivedMessageQueue.Enqueue(msg);
                                    break;
                                }
                        }
                    }

                    await Task.Delay(1);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, $"Client failure: {nameof(TcpOttdClient)}:{nameof(MainLoop)} for {serverIp}:{serverPort}");
                    this.Reset();
#if DEBUG
                    await Task.Delay(5_000); // wait small amount of time xD before reconnecting.
#else
                    await Task.Delay(60_000); // wait 60 seconds before reconnecting.
#endif
                }
            }

            this.ConnectionState = ConnectionState.NotConnected;
        }

        private void Reset()
        {
            this.client = new TcpClient();
            this.players.Clear();
            this.sendMessageQueue.Clear();
            this.receivedMessageQueue.Clear();
            this.MyClientId = 0;
            this.connected = false;
            this.ConnectionState = ConnectionState.NotConnected;
        }

        public async Task Start(string serverIp, int serverPort, string username, string password, string revision, uint newgrfRevision)
        {
            if (this.ConnectionState != ConnectionState.Idle)
            {
                throw new OttdException("You cannot connect to different server while this client is connected to a server");
            }
            this.ConnectionState = ConnectionState.NotConnected;


            this.logger.LogInformation($"Connecting to {serverIp}:{serverPort}");
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(cancellationTokenSource.Token, serverIp, serverPort, username, password, revision, newgrfRevision)), null);
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => UpdateEvents(cancellationTokenSource.Token)), null);

            if ((await TaskHelper.WaitUntil(() => ConnectionState == ConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))) == false)
            {
                try
                {
                    await this.Stop();
                }
                finally
                {
                    throw new OttdConnectionException("Could not connect");
                }
            }
        }

        public async Task Stop()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
            await TaskHelper.WaitUntil(() => ConnectionState == ConnectionState.NotConnected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10));
            this.ConnectionState = ConnectionState.Idle;
        }
    }
}
