﻿using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Reporting
{
    public class ReportService : IAdminPortClientUser, IReportService
    {
        private readonly IReportServerService reportServerService;
        private readonly IAdminPortClientProvider clientProvider;
        private readonly DiscordSocketClient discord;
        private readonly ILogger<ReportService> logger;
        private readonly Random random = new Random();

        private ConcurrentDictionary<string, ReportServerInfo> ReportServers { get; } = new ConcurrentDictionary<string, ReportServerInfo>();
        private ConcurrentQueue<ReportServer> ServersToRemove { get; } = new ConcurrentQueue<ReportServer>();
        private ConcurrentQueue<ServerEvent> ReceivedEvents { get; } = new ConcurrentQueue<ServerEvent>();

        public ReportService(IReportServerService reportServerService, IAdminPortClientProvider adminPortClientProvider, DiscordSocketClient discord, ILogger<ReportService> logger)
        {
            this.reportServerService = reportServerService;
            this.clientProvider = adminPortClientProvider;
            this.logger = logger;
            this.discord = discord;
        }

        public async Task Start()
        {
            var reportServers = await this.reportServerService.GetAll();

            foreach (var r in reportServers)
                ReportServers.TryAdd(r.Server.GetUniqueKey(), new ReportServerInfo(r));

            this.reportServerService.Added += (_, r) => ReportServers.TryAdd(r.Server.GetUniqueKey(), new ReportServerInfo(r));
            this.reportServerService.Removed += (_, r) => ServersToRemove.Enqueue(r);

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
        }

        public async void MainLoop()
        {
            logger.LogInformation("Starting report service");
            while (true)
            {
                try
                {
#if DEBUG
                    await Task.Delay(TimeSpan.FromSeconds(0.05));
#else
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
#endif

                    await RemoveServers();
                    await JoinUnjoinedClients();
                    await HandleEvents();

                }
                catch (Exception e)
                {
                    logger.LogError($"Error {e.Message}", e);
                }
            }
        }

        private async Task HandleEvents()
        {
            while (ReceivedEvents.TryDequeue(out var eventInfo))
                if (ReportServers.TryGetValue(eventInfo.Server.GetUniqueKey(), out ReportServerInfo rso))
                {
                    var client = clientProvider.GetClient(this, rso.ReportServer.Server);
                    if (eventInfo.AdminEvent.EventType == AdminEventType.ChatMessageReceived)
                    {
                        var chatMsg = eventInfo.AdminEvent as AdminChatMessageEvent;
                        rso.AddMessage($"[{DateTimeOffset.Now:HH:mm zz}] {chatMsg.Player.Name} : {chatMsg.Message}");
                        if (chatMsg.Message.StartsWith("!report"))
                        {
                            logger.LogInformation($"{chatMsg.Player.Name} started report process on {rso.ReportServer.Server.ServerName}");
                            string reason = string.Empty;
                            var parts = chatMsg.Message.Split("!report");
                            if (parts.Count() > 1)
                                reason = parts[1];

                            rso.ServerState = ReportServerState.GatheringClientList;
                            rso.Report = new ReportMessage(rso.ReportServer.Server, chatMsg.Player.Name, reason);
                            rso.CurrentNewSection = new ReportSection("Connected clients");
                            client.SendMessage(new AdminRconMessage("clients"));
                            rso.CurrentPingValue = (uint)random.Next(0, 1_000_000);
                            client.SendMessage(new AdminPingMessage(rso.CurrentPingValue));
                        }
                    }
                    else if (rso.ServerState != ReportServerState.Listening && eventInfo.AdminEvent is AdminPongEvent pongEvent)
                    {
                        if (rso.CurrentPingValue == pongEvent.PongValue)
                        {
                            if (rso.ServerState == ReportServerState.GatheringClientList)
                            {
                                logger.LogInformation($"Starting to gather companies on {rso.ReportServer.Server.ServerName}");
                                rso.ServerState = ReportServerState.GatheringCompanies;
                                rso.Report.AddSection(rso.CurrentNewSection);
                                rso.CurrentNewSection = new ReportSection("Companies");
                                client.SendMessage(new AdminRconMessage("companies"));
                                rso.CurrentPingValue = (uint)random.Next(0, 1_000_000);
                                client.SendMessage(new AdminPingMessage(rso.CurrentPingValue));

                            }
                            else if (rso.ServerState == ReportServerState.GatheringCompanies)
                            {
                                rso.ServerState = ReportServerState.Listening;
                                logger.LogInformation($"Generating report on  {rso.ReportServer.Server.ServerName}");
                                rso.Report.AddSection(rso.CurrentNewSection);

                                var chatSection = new ReportSection("Chat");
                                foreach(var chat in rso.LastMessages)
                                {
                                    chatSection.AddData(chat);
                                }

                                rso.Report.AddSection(chatSection);
                                
                                // create file and send to chat.

                                string tmpFile = Path.GetTempFileName();
                                try
                                {
                                    using (StreamWriter w = new StreamWriter(tmpFile))
                                    {
                                        ReportWriter reportWriter = new ReportWriter(w);
                                        await reportWriter.WriteReport(rso.Report);
                                    }

                                    var channel = discord.GetChannel(rso.ReportServer.ChannelId) as SocketTextChannel;
                                    using (FileStream fs = new FileStream(tmpFile, FileMode.Open))
                                    {
                                        await channel.SendFileAsync(fs, $"{rso.ReportServer.Server.ServerName}-{DateTime.Now}-{rso.Report.ReporterName}.txt", $"[{rso.ReportServer.Server.ServerName}] {rso.Report.ReporterName} - report");
                                    }
                                    logger.LogInformation($"Report generated on  {rso.ReportServer.Server.ServerName}");

                                }
                                catch (Exception e)
                                {
                                    logger.LogError($"{rso.ReportServer.Server.ServerName} - Problem with generating error - {e}");
                                }
                                finally
                                {
                                    File.Delete(tmpFile);
                                }
                            }
                        }
                    }
                    else if (rso.ServerState != ReportServerState.Listening && eventInfo.AdminEvent is AdminRconEvent rconEvent)
                    {
                        rso.CurrentNewSection.AddData(rconEvent.Message);
                    }
                }
        }

        private async Task JoinUnjoinedClients()
        {
            foreach (var r in ReportServers.Values)
            {
                if (clientProvider.IsRegistered(this, r.ReportServer.Server) == false)
                {
                    await clientProvider.Register(this, r.ReportServer.Server);
                }
            }
        }

        private async Task RemoveServers()
        {
            while (this.ServersToRemove.TryDequeue(out ReportServer r))
            {
                await this.clientProvider.Unregister(this, r.Server);
                this.ReportServers.TryRemove(r.Server.GetUniqueKey(), out _);
            }
        }




        public void ParseServerEvent(Server server, IAdminEvent adminEvent) =>
            this.ReceivedEvents.Enqueue(new ServerEvent(server, adminEvent));
    }
}
