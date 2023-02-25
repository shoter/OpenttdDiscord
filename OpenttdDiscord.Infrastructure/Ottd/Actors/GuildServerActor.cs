﻿using Akka.Actor;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Servers;
using OpenttdDiscord.Infrastructure.Statuses.Actors;
using OpenttdDiscord.Infrastructure.Statuses.Messages;
using OpenttdDiscord.Infrastructure.Statuses.UseCases;
using Serilog;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal class GuildServerActor : ReceiveActorBase, IWithTimers
    {
        private readonly OttdServer server;
        private readonly AdminPortClient client;
        private readonly IGetStatusMonitorsForServerUseCase getStatusMonitorsForServerUseCase;
        private readonly IUpdateStatusMonitorUseCase updateStatusMonitorUseCase;
        private readonly ExtDictionary<ulong, IActorRef> statusMonitorActors = new();

        public ITimerScheduler Timers { get; set; } = default!;

        public GuildServerActor(IServiceProvider serviceProvider, OttdServer server) : base(serviceProvider)
        {
            this.server = server;
            this.getStatusMonitorsForServerUseCase = SP.GetRequiredService<IGetStatusMonitorsForServerUseCase>();
            this.updateStatusMonitorUseCase = SP.GetRequiredService<IUpdateStatusMonitorUseCase>();
            client = new(new AdminPortClientSettings()
            {
                WatchdogInterval = TimeSpan.FromSeconds(5)
            },
            new(server.Ip, server.AdminPort, server.AdminPortPassword),
            logging => logging.AddSerilog());

            Ready();
            Self.Tell(new InitGuildServerActorMessage());
        }

        private void Ready()
        {
            ReceiveAsync<InitGuildServerActorMessage>(InitGuildServerActorMessage);
            Receive<ExecuteServerAction>(ExecuteServerAction);
            Receive<KillDanglingAction>(KillDanglingAction);
            Receive<RegisterStatusMonitor>(RegisterStatusMonitor);
            ReceiveAsync<RemoveStatusMonitor>(RemoveStatusMonitor);
            ReceiveAsync<UpdateStatusMonitor>(UpdateStatusMonitor);
        }

        public static Props Create(IServiceProvider sp, OttdServer server)
            => Props.Create(() => new GuildServerActor(sp, server));

        protected override void PostStop()
        {
            client.Disconnect().Wait();
            base.PostStop();
        }

        private async Task InitGuildServerActorMessage(InitGuildServerActorMessage _)
        {
            logger.LogInformation($"Connecting to {server.Name} on {server.Ip}:{server.AdminPort}");
            await client.Connect();

            var monitors = (await getStatusMonitorsForServerUseCase.Execute(User.Master, server.Id))
                .ThrowIfError()
                .Right();

            foreach(var monitor in monitors)
            {
                CreateStatusMonitor(monitor);
                logger.LogInformation($"Created monitor for {server.Name} on channel {monitor.ChannelId}");
            }
        }

        private void ExecuteServerAction(ExecuteServerAction cmd)
        {
            Props props = cmd.CreateCommandActorProps(SP, server, client);
            var commandActor = Context.ActorOf(props);
            Timers.StartSingleTimer(commandActor, new KillDanglingAction(commandActor), cmd.TimeOut);
            commandActor.Tell(cmd);
        }

        private void KillDanglingAction(KillDanglingAction msg)
        {
            msg.commandActor.GracefulStop(TimeSpan.FromSeconds(1));
        }

        private void RegisterStatusMonitor(RegisterStatusMonitor msg)
        {
            CreateStatusMonitor(msg.StatusMonitor);
            Sender.Tell(Unit.Default);
        }

        private EitherUnit CreateStatusMonitor(StatusMonitor monitor)
        {
            Props props = StatusMonitorActor.Create(server, monitor, client, SP);
            IActorRef actor = Context.ActorOf(props);
            statusMonitorActors.Add(monitor.ChannelId, actor);
            return Unit.Default;
        }

        private async Task RemoveStatusMonitor(RemoveStatusMonitor rmv)
        {
            var statusMonitor = statusMonitorActors.TryGetValueAs<IActorRef>(rmv.ChannelId);

            if(statusMonitor.IsNone)
            {
                throw new ActorNotFoundException();
            }

            var actor = statusMonitor.Value();
            actor.Forward(rmv);
            await actor.GracefulStop(TimeSpan.FromSeconds(1));
            statusMonitorActors.Remove(rmv.ChannelId);
            Sender.Tell(Unit.Default);
        }

        private async Task UpdateStatusMonitor(UpdateStatusMonitor usm)
        {
            await this.RemoveStatusMonitor(new(usm.UpdatedMonitor.ServerId, usm.UpdatedMonitor.GuildId, usm.UpdatedMonitor.ChannelId));
            var self = Self;
            var monitorResult = await this.updateStatusMonitorUseCase.Execute(User.Master, usm.UpdatedMonitor);
            monitorResult.Map(monitor =>
            {
                self.Tell(new RegisterStatusMonitor(monitor));
                return Unit.Default;
            });
        }
    }
}
