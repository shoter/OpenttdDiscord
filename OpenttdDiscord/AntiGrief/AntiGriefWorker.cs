using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Discord.WebSocket;

using Microsoft.Extensions.Logging;

using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;

using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Chatting;
using OpenttdDiscord.Database.AntiGrief;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.AntiGrief
{
    public class AntiGriefWorker : IAdminPortClientUser, IAntiGriefWorker
    {
        private readonly ILogger<AntiGriefWorker> logger;
        private readonly IAntiGriefService antiGriefService;
        private readonly ITrustedIpService trustedIpService;
        private readonly IAdminPortClientProvider adminPortClientProvider;

        private readonly ConcurrentDictionary<string, AntiGriefServer> agServers = new();
        private readonly ConcurrentQueue<ServerEvent> receivedEvents = new();
        private readonly ConcurrentQueue<AntiGriefServer> serversToRemove = new();
        private readonly ConcurrentStack<ReputationAdd> ReputationToAdd = new();

        private DateTimeOffset LastReputationTime = DateTimeOffset.Now;

        public AntiGriefWorker(ILogger<AntiGriefWorker> logger, IAdminPortClientProvider adminPortClientProvider, IAntiGriefService antiGriefService, ITrustedIpService trustedIpService)
        {
            this.logger = logger;
            this.adminPortClientProvider = adminPortClientProvider;
            this.antiGriefService = antiGriefService;
            this.trustedIpService = trustedIpService;
        }

        public async Task Start()
        {
            foreach (var s in await this.antiGriefService.GetAll())
            {
                this.agServers.TryAdd(s.Server.GetUniqueKey(), s);
            }


            this.antiGriefService.Added += (_, s) => agServers.AddOrUpdate(s.Server.GetUniqueKey(), s, (_, __) => s);
            this.antiGriefService.Removed += (_, s) => serversToRemove.Enqueue(s);

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => ReputationLoop()), null);

        }

        public async void ReputationLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                if (ReputationToAdd.TryPop(out ReputationAdd result))
                {
                    TrustedIp ip = await this.trustedIpService.Get(result.IpAddres);
                    if (ip != null)
                    {
                        await this.trustedIpService.Remove(ip);
                    }

                    TimeSpan playingTime = ip?.PlayingTime ?? TimeSpan.Zero;

                    await this.trustedIpService.Add(new TrustedIp(result.IpAddres, playingTime.Add(TimeSpan.FromMinutes(result.Minutes))));
                }
            }
        }

        public async void MainLoop()
        {
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
                    AddPlayerReputation();
                    await HandleGameMessages();

                }
                catch (Exception e)
                {
                    logger.LogError($"Error {e.Message}", e);
                }
            }
        }

        private void AddPlayerReputation()
        {
            if ((DateTimeOffset.Now - LastReputationTime).TotalMinutes >= 1f)
            {
                foreach (var pair in this.agServers)
                {
                    var s = pair.Value.Server;
                    IAdminPortClient client = adminPortClientProvider.GetClient(this, s);
                    foreach (var p in client.Players.Values)
                    {
                        this.logger.LogDebug($"Adding reputation for {p.Name} - {p.Hostname}");
                        this.ReputationToAdd.Push(new ReputationAdd(p.Hostname, 1));
                    }
                }

                LastReputationTime = DateTimeOffset.Now;
            }
        }

        private async Task HandleGameMessages()
        {
            while (receivedEvents.TryDequeue(out ServerEvent serverEvent))
            {
                Player player = null;
                if (serverEvent.AdminEvent is AdminClientUpdateEvent msg)
                {
                    player = msg.Player;

                }
                else if (serverEvent.AdminEvent is AdminClientInfoEvent info)
                {
                    player = info.Player;

                }

                if(player == null)
                {
                    continue;
                }

                if (player.ClientId == 1)
                    continue;

                AntiGriefServer s = agServers.Values.FirstOrDefault(c => c.Server.ServerIp == serverEvent.Server.ServerIp && c.Server.ServerPort == serverEvent.Server.ServerPort);

                if (s == null)
                    continue;

                TrustedIp tip = await trustedIpService.Get(player.Hostname);

                if (tip == null)
                {
                    tip = await trustedIpService.Add(new TrustedIp(player.Hostname, TimeSpan.Zero));
                }

                var client = adminPortClientProvider.GetClient(this, s.Server);

                if (s.RequiredMinsToPlay > tip.PlayingTime.TotalMinutes)
                {
                    int minutesRequired = (int)(s.RequiredMinsToPlay - tip.PlayingTime.TotalMinutes);

                    client.SendMessage(new AdminRconMessage($"move {player.ClientId} 255"));
                    client.SendMessage(new AdminRconMessage($"say_client {player.ClientId} \"You need {minutesRequired} minutes to be able to join companies. {s.Reason}\""));
                }

            }
        }

        private async Task JoinUnjoinedClients()
        {
            foreach (var s in agServers.Values)
            {
                try
                {
                    if (adminPortClientProvider.IsRegistered(this, s.Server) == false)
                    {
                        await adminPortClientProvider.Register(this, s.Server);
                        var client = adminPortClientProvider.GetClient(this, s.Server);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError($"{s.Server.ServerIp}:{s.Server.ServerPort.ToString()} - cannot join - {e.Message}");
                }
            }
        }

        private async Task RemoveServers()
        {
            while (serversToRemove.TryDequeue(out var s))
            {
                await adminPortClientProvider.Unregister(this, s.Server);
                agServers.Remove(s.Server.GetUniqueKey(), out _);
            }
        }

        public void ParseServerEvent(Server server, IAdminEvent adminEvent)
        {
            this.receivedEvents.Enqueue(new ServerEvent(server, adminEvent));
        }
    }
}
