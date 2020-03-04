﻿using Microsoft.Extensions.Logging;
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
        private readonly IRevisionTranslator revisionTranslator;
        private readonly TcpClient client = new TcpClient();
        private bool connected = false;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private ConnectionState connectionState = ConnectionState.NotConnected;

        private readonly ConcurrentDictionary<uint, Player> players = new ConcurrentDictionary<uint, Player>();

        private readonly TcpMessageType[] ignoredLoggedTypes = new TcpMessageType[]
        {
                TcpMessageType.PACKET_SERVER_SYNC,
                TcpMessageType.PACKET_CLIENT_ACK,
                TcpMessageType.PACKET_SERVER_FRAME
        };

        private uint myClientId = 0;
        public TcpOttdClient(ITcpPacketCreator packetCreator, ITcpPacketReader packetReader, IUdpPacketReader udpPacketReader, IUdpPacketCreator udpPacketCreator, IRevisionTranslator revisionTranslator, ILogger<ITcpOttdClient> logger)
        {
            this.logger = logger;
            this.packetCreator = packetCreator;
            this.packetReader = packetReader;
            this.udpPacketCreator = udpPacketCreator;
            this.udpPacketReader = udpPacketReader;
            this.revisionTranslator = revisionTranslator;
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

        public async void UpdateEvents(CancellationToken token)
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

        public async void MainLoop(CancellationToken token, string serverIp, int serverPort, string username, string password)
        {

            Task sizeTask = null;
            byte[] sizeBuffer = new byte[2];

            while (token.IsCancellationRequested == false)
            {
                try
                {
                    if (this.connectionState == ConnectionState.NotConnected)
                    {
                        client.Connect(serverIp, serverPort);
                        this.QueueInternal(await this.CreateJoinMessage(serverIp, serverPort, username));
                        this.connectionState = ConnectionState.Connecting;
                    }
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
                                        this.QueueInternal(new PacketClientGamePasswordMessage(password));
                                        break;
                                    }
                                case TcpMessageType.PACKET_SERVER_FRAME:
                                    {
                                        var m = msg as PacketServerFrameMessage;
                                        this.QueueInternal(new PacketClientAckMessage(m.FrameCounter, m.Token));
                                        this.connected = true;
                                        this.connectionState = ConnectionState.Connected;
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
                                        this.connectionState = ConnectionState.DownloadingMap;
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
        }

        private void Reset()
        {
            this.players.Clear();
            this.internalSendMessageQueue.Clear();
            this.sendMessageQueue.Clear();
            this.receivedMessageQueue.Clear();
            this.myClientId = 0;
            this.connected = false;
            this.connectionState = ConnectionState.NotConnected;
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
                NewgrfVersion = this.revisionTranslator.TranslateToNewGrfRevision(revision).Revision
            };
        }

        public Task Start(string serverIp, int serverPort, string username, string password = "")
        {
            this.logger.LogInformation($"Connecting to {serverIp}:{serverPort}");
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(cancellationTokenSource.Token, serverIp, serverPort, username, password)), null);
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => UpdateEvents(cancellationTokenSource.Token)), null);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            this.cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
