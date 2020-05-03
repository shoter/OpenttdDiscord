using Microsoft.Extensions.Logging;
using OpenttdDiscord.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Network.AdminPort
{
    public class AdminPortClient : IAdminPortClient
    {
        private TcpClient tcpClient;
        public AdminConnectionState ConnectionState { get; private set; }

        public ConcurrentDictionary<uint, Player> Players { get; } = new ConcurrentDictionary<uint, Player>();

        public event EventHandler<IAdminEvent> EventReceived;

        private readonly Microsoft.Extensions.Logging.ILogger logger;

        private readonly IAdminPacketService adminPacketService;

        private readonly IAdminMessageProcessor messageProcessor;

        private readonly ConcurrentQueue<IAdminMessage> receivedMessagesQueue = new ConcurrentQueue<IAdminMessage>();

        private readonly ConcurrentQueue<IAdminMessage> sendMessageQueue = new ConcurrentQueue<IAdminMessage>();

        

        private CancellationTokenSource cancellationTokenSource = null;

        public ServerInfo ServerInfo { get; }

        public ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting> AdminUpdateSettings { get; } = new ConcurrentDictionary<AdminUpdateType, AdminUpdateSetting>();


        public AdminServerInfo AdminServerInfo { get; private set; } = new AdminServerInfo();
        public AdminPortClient(ServerInfo serverInfo, IAdminPacketService adminPacketService, IAdminMessageProcessor messageProcessor, ILogger<IAdminPortClient> logger)
        {
            this.ServerInfo = serverInfo;
            this.logger = logger;
            this.adminPacketService = adminPacketService;
            this.messageProcessor = messageProcessor;

            foreach(var type in Enums.ToArray<AdminUpdateType>())
            {
                this.AdminUpdateSettings.TryAdd(type, new AdminUpdateSetting(false, type, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
            }
        }

        private async void EventLoop(CancellationToken token)
        {
            while(token.IsCancellationRequested == false)
            {
                if(this.receivedMessagesQueue.TryDequeue(out IAdminMessage msg))
                {
                    var eventMessage = this.messageProcessor.ProcessMessage(msg, this);

                    if(eventMessage != null)
                        this.EventReceived?.Invoke(this,eventMessage );
                }

                await Task.Delay(TimeSpan.FromSeconds(0.1));
            }

        }

        private async void MainLoop(CancellationToken token)
        {
            Task<int> sizeTask = null;
            byte[] sizeBuffer = new byte[2];

            while (token.IsCancellationRequested == false)
            {
                try
                {
                    if (this.ConnectionState == AdminConnectionState.NotConnected)
                    {
                        tcpClient = new TcpClient();
                        tcpClient.ReceiveTimeout = 2000;
                        tcpClient.SendTimeout = 2000;
                        tcpClient.Connect(ServerInfo.ServerIp, ServerInfo.ServerPort);
                        this.SendMessage(new AdminJoinMessage(ServerInfo.Password, "OttdBot", "1.0.0"));
                        logger.LogInformation($"{ServerInfo} Connecting");

                        this.ConnectionState = AdminConnectionState.Connecting;
                    }

                    if (this.tcpClient == null)
                        continue;

                    for(int i = 0;i < 100; ++i)
                    {
                        if (this.sendMessageQueue.TryDequeue(out IAdminMessage msg))
                        {
                            logger.LogInformation($"{ServerInfo} sent {msg.MessageType}");
                            Packet packet = this.adminPacketService.CreatePacket(msg);
                            await tcpClient.GetStream().WriteAsync(packet.Buffer, 0, packet.Size).WaitMax(TimeSpan.FromSeconds(2));
                        }
                        else
                            break;
                    }

                    while ((sizeTask ??= tcpClient.GetStream().ReadAsync(sizeBuffer, 0, 2)).IsCompleted)
                    {
                        var receivedBytes = sizeTask.Result;

                        if(receivedBytes != 2)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(1));
                            int bytes = await tcpClient.GetStream().ReadAsync(sizeBuffer, 1, 1).WaitMax(TimeSpan.FromSeconds(2));
                            if (bytes == 0)
                            {
                                throw new OttdConnectionException("Something went wrong - restarting");
                            }

                        }

                        sizeTask = null;

                        ushort size = BitConverter.ToUInt16(sizeBuffer, 0);

                        byte[] content = new byte[size];
                        content[0] = sizeBuffer[0];
                        content[1] = sizeBuffer[1];
                        int contentSize = 2;

                        do
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(1));
                            Task<int> task = tcpClient.GetStream().ReadAsync(content, contentSize, size - contentSize).WaitMax(TimeSpan.FromSeconds(2), $"{ServerInfo} no data received");
                            await task;
                            contentSize += task.Result;
                            if(task.Result == 0)
                            {
                                throw new OttdConnectionException("No further data received in message!");
                            }
                        } while (contentSize < size);


                        var packet = new Packet(content);
                        IAdminMessage message = this.adminPacketService.ReadPacket(packet);
                        if (message == null)
                            break;

                        this.logger.LogInformation($"{ServerInfo} received {message.MessageType}");

                        switch (message.MessageType)
                        {
                            case AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL:
                                {
                                    var msg = message as AdminServerProtocolMessage;

                                    foreach(var s in msg.AdminUpdateSettings)
                                    {
                                        this.logger.LogInformation($"Update settings {s.Key} - {s.Value}");
                                        this.AdminUpdateSettings.TryUpdate(s.Key, new AdminUpdateSetting(true, s.Key, s.Value), this.AdminUpdateSettings[s.Key]);
                                    }

                                    break;
                                }
                            case AdminMessageType.ADMIN_PACKET_SERVER_WELCOME:
                                {
                                    var msg = message as AdminServerWelcomeMessage;

                                    AdminServerInfo = new AdminServerInfo()
                                    {
                                        IsDedicated = msg.IsDedicated,
                                        MapName = msg.MapName,
                                        RevisionName = msg.NetworkRevision,
                                        ServerName = msg.ServerName
                                    };


                                    this.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CHAT, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
                                    this.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CONSOLE, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
                                    this.SendMessage(new AdminUpdateFrequencyMessage(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, UpdateFrequency.ADMIN_FREQUENCY_AUTOMATIC));
                                    this.SendMessage(new AdminPollMessage(AdminUpdateType.ADMIN_UPDATE_CLIENT_INFO, uint.MaxValue));

                                    this.ConnectionState = AdminConnectionState.Connected;
                                    this.logger.LogInformation($"{ServerInfo.ServerIp}:{ServerInfo.ServerPort} - connected");

                                    break;
                                }
                            case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_INFO:
                                {
                                    var msg = message as AdminServerClientInfoMessage;
                                    var player = new Player(msg.ClientId, msg.ClientName);
                                    this.Players.AddOrUpdate(msg.ClientId, player, (_, __) => player);

                                    break;
                                }
                            case AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_UPDATE:
                                {
                                    var msg = message as AdminServerClientUpdateMessage;
                                    var player = this.Players[msg.ClientId];
                                    player.Name = msg.ClientName;

                                    break;
                                }
                            default:
                                {
                                    var msg = message as AdminServerChatMessage;
                                    this.receivedMessagesQueue.Enqueue(message);
                                    break;
                                }
                        }

                    }



                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                }
                catch(Exception e)
                {
                    this.logger.LogError($"{ServerInfo.ServerIp}:{ServerInfo.ServerPort} encountered error {e.Message}", e);

                    this.tcpClient?.Dispose();
                    this.tcpClient = null;
                    this.sendMessageQueue.Clear();
                    this.receivedMessagesQueue.Clear();
                    sizeTask = null;
                    this.ConnectionState = AdminConnectionState.NotConnected;

                    await Task.Delay(TimeSpan.FromSeconds(30));
                }

            }

            this.logger.LogInformation($"{ServerInfo} disconnected");
            this.ConnectionState = AdminConnectionState.Idle;
        }

        public async Task Join()
        {
            if (this.ConnectionState != AdminConnectionState.Idle)
                return;

            try
            {
                this.ConnectionState = AdminConnectionState.NotConnected;
                this.cancellationTokenSource = new CancellationTokenSource();

                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(cancellationTokenSource.Token)), null);
                ThreadPool.QueueUserWorkItem(new WaitCallback((_) => EventLoop(cancellationTokenSource.Token)), null);

                if (!(await TaskHelper.WaitUntil(() => ConnectionState == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
                {
                    this.cancellationTokenSource.Cancel();
                    this.cancellationTokenSource = new CancellationTokenSource();
                    throw new OttdConnectionException("Admin port could not connect to the server");
                }
            }
            catch(Exception e)
            {
                this.ConnectionState = AdminConnectionState.Idle;
                throw new OttdConnectionException("Could not join server", e);
            }
        }

        public async Task Disconnect()
        {
            if (this.ConnectionState == AdminConnectionState.Idle)
                return;
            try
            {
                this.cancellationTokenSource.Cancel();

                if (!(await TaskHelper.WaitUntil(() => ConnectionState == AdminConnectionState.Idle, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
                {
                    this.cancellationTokenSource = new CancellationTokenSource();
                }
            }
            catch (Exception e)
            {
                throw new OttdConnectionException("Error during stopping server", e);
            }
        }

        public void SendMessage(IAdminMessage message)
        {
            this.sendMessageQueue.Enqueue(message);
        }
    }
}
