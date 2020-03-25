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

        public event EventHandler<IAdminMessage> MessageReceived;

        private readonly Microsoft.Extensions.Logging.ILogger logger;

        private readonly IAdminPacketService adminPacketService;

        private readonly ConcurrentQueue<IAdminMessage> receivedMessagesQueue = new ConcurrentQueue<IAdminMessage>();

        private readonly ConcurrentQueue<IAdminMessage> sendMessageQueue = new ConcurrentQueue<IAdminMessage>();

        private CancellationTokenSource cancellationTokenSource = null;

        public ServerInfo ServerInfo { get; }

        public ConcurrentDictionary<AdminUpdateType, bool> HandledUpdateTypes { get; } = new ConcurrentDictionary<AdminUpdateType, bool>();
        public AdminPortClient(ServerInfo serverInfo, IAdminPacketService adminPacketService, ILogger<AdminPortClient> logger)
        {
            this.ServerInfo = serverInfo;
            this.logger = logger;
            this.adminPacketService = adminPacketService;

            foreach(var type in Enums.ToArray<AdminUpdateType>())
            {
                this.HandledUpdateTypes.TryAdd(type, false);
            }
        }

        private void EventLoop(CancellationToken token)
        {

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
                        tcpClient = new TcpClient(ServerInfo.ServerIp, ServerInfo.ServerPort);
                        this.SendMessage(new AdminJoinMessage(ServerInfo.Password, "OttdBot", "1.0.0"));
                        this.ConnectionState = AdminConnectionState.Connecting;
                    }

                    for(int i = 0;i < 100; ++i)
                    {
                        if (this.sendMessageQueue.TryDequeue(out IAdminMessage msg))
                        {
                            Packet packet = this.adminPacketService.CreatePacket(msg);
                            await tcpClient.GetStream().WriteAsync(packet.Buffer, 0, packet.Size);
                        }
                        else
                            break;
                    }

                    while ((sizeTask ??= tcpClient.GetStream().ReadAsync(sizeBuffer, 0, 2)).IsCompleted)
                    {
                        var receivedBytes = sizeTask.Result;

                        if(receivedBytes != 2)
                        {
                            await tcpClient.GetStream().ReadAsync(sizeBuffer, 1, 1);
                        }

                        sizeTask = null;

                        ushort size = BitConverter.ToUInt16(sizeBuffer, 0);
                        byte[] content = new byte[size];
                        content[0] = sizeBuffer[0];
                        content[1] = sizeBuffer[1];
                        int contentSize = 2;

                        do
                        {
                            Task<int> task = tcpClient.GetStream().ReadAsync(content, contentSize, size - contentSize);
                            await task;
                            contentSize += task.Result;
                        } while (contentSize < size);


                        var packet = new Packet(content);
                        IAdminMessage message = this.adminPacketService.ReadPacket(packet);

                        switch(message.MessageType)
                        {
                            case AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL:
                                {
                                    var msg = message as AdminServerProtocolMessage;

                                    foreach(var s in msg.AdminUpdateSettings)
                                    {

                                    }


                                }
                        }

                    }



                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                }
                catch(Exception e)
                {
                    this.tcpClient.Dispose();
                    this.tcpClient = null;
                    this.sendMessageQueue.Clear();
                    this.receivedMessagesQueue.Clear();
                    this.logger.LogError($"{ServerInfo.ServerIp}:{ServerInfo.ServerPort} encountered error", e);

                }

            }

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

                if ((await TaskHelper.WaitUntil(() => ConnectionState == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))) == false)
                {
                    this.cancellationTokenSource = new CancellationTokenSource();
                    this.cancellationTokenSource.Cancel();
                }




                    throw new NotImplementedException();
            }
            catch(Exception)
            {
                this.ConnectionState = AdminConnectionState.Idle;
            }
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IAdminMessage message)
        {
            this.sendMessageQueue.Enqueue(message);
        }
    }
}
