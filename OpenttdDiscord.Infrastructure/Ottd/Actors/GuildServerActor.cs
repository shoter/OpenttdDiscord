using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Statuses;
using OpenttdDiscord.Domain.Statuses.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting;
using OpenttdDiscord.Infrastructure.Chatting.Actors;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Statuses.Actors;
using OpenttdDiscord.Infrastructure.Statuses.Messages;
using Serilog;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal partial class GuildServerActor : ReceiveActorBase, IWithTimers
    {
        private readonly OttdServer server;
        private readonly AdminPortClient client;
        private readonly IAkkaService akkaService;
        private readonly IGetStatusMonitorsForServerUseCase getStatusMonitorsForServerUseCase;
        private readonly IUpdateStatusMonitorUseCase updateStatusMonitorUseCase;
        private readonly IGetChatChannelUseCase getChatChannelUseCase;
        private readonly ExtDictionary<ulong, IActorRef> statusMonitorActors = new();
        private readonly ExtDictionary<ulong, ChattingActors> chatChannelActors = new();
        private readonly System.Collections.Generic.HashSet<IActorRef> adminEventSubscribers = new();

        public ITimerScheduler Timers { get; set; } = default!;

        public GuildServerActor(
            IServiceProvider serviceProvider,
            OttdServer server)
            : base(serviceProvider)
        {
            this.server = server;

            this.getStatusMonitorsForServerUseCase = SP.GetRequiredService<IGetStatusMonitorsForServerUseCase>();
            this.updateStatusMonitorUseCase = SP.GetRequiredService<IUpdateStatusMonitorUseCase>();
            this.getChatChannelUseCase = SP.GetRequiredService<IGetChatChannelUseCase>();
            this.akkaService = SP.GetRequiredService<IAkkaService>();

            client = new(new AdminPortClientSettings()
            {
                WatchdogInterval = TimeSpan.FromSeconds(5),
            },
            new(server.Ip, server.AdminPort, server.AdminPortPassword),
            logging => logging.AddSerilog());

            RconConstructor();

            Ready();
            RconReady();
            Self.Tell(new InitGuildServerActorMessage());
        }

        private void Ready()
        {
            ReceiveAsync<InitGuildServerActorMessage>(InitGuildServerActorMessage);
            Receive<ExecuteServerAction>(ExecuteServerAction);
            Receive<RegisterStatusMonitor>(RegisterStatusMonitor);
            ReceiveAsync<RemoveStatusMonitor>(RemoveStatusMonitor);
            ReceiveAsync<UpdateStatusMonitor>(UpdateStatusMonitor);
            Receive<RegisterChatChannel>(RegisterChatChannel);
            ReceiveAsync<UnregisterChatChannel>(UnregisterChatChannel);

            Receive<KillDanglingAction>(msg => msg.commandActor.GracefulStop(TimeSpan.FromSeconds(1)));
            Receive<IAdminEvent>(ev => adminEventSubscribers.TellMany(ev));
            Receive<SubscribeToAdminEvents>(m => adminEventSubscribers.Add(m.Subscriber));
            Receive<UnsubscribeFromAdminEvents>(m => adminEventSubscribers.Remove(m.subscriber));
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

            foreach (var monitor in monitors)
            {
                CreateStatusMonitor(monitor);
                logger.LogInformation($"Created monitor for {server.Name} on channel {monitor.ChannelId}");
            }

            var chatChannels = (await getChatChannelUseCase.Execute(User.Master, server.Id))
                .ThrowIfError()
                .Right();

            foreach (var cc in chatChannels)
            {
                RegisterChatChannel(new(cc));
            }

            client.SetAdminEventHandler(ev => self.Tell(ev));

            await RconInit();
        }

        private void ExecuteServerAction(ExecuteServerAction cmd)
        {
            Props props = cmd.CreateCommandActorProps(SP, server, client);
            var commandActor = Context.ActorOf(props);
            Timers.StartSingleTimer(commandActor, new KillDanglingAction(commandActor), cmd.TimeOut);
            commandActor.Tell(cmd);
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

            if (statusMonitor.IsNone)
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

        private void RegisterChatChannel(RegisterChatChannel rcc)
        {
            if (chatChannelActors.ContainsKey(rcc.chatChannel.ChannelId))
            {
                logger.LogWarning($"Chat channel {server.Name} - {rcc.chatChannel.ChannelId} already registered");
                return;
            }

            var discord = Context.ActorOf(DiscordCommunicationActor.Create(SP, rcc.chatChannel.ChannelId, client, server), "discordCommunication");
            var openttd = Context.ActorOf(OttdCommunicationActor.Create(SP, rcc.chatChannel.ChannelId, server, client), "ottdCommunication");

            var chattingActors = new ChattingActors(rcc.chatChannel, discord, openttd);
            chatChannelActors.Add(rcc.chatChannel.ChannelId, chattingActors);
            logger.LogInformation($"Registered chat channel {rcc.chatChannel.ChannelId} for {server.Name}");
        }

        private async Task UnregisterChatChannel(UnregisterChatChannel ucc)
        {
            if (!chatChannelActors.ContainsKey(ucc.ChannelId))
            {
                logger.LogError($"Chat channel {server.Name} - {ucc.ChannelId} already removed");
                return;
            }

            ChattingActors actors = chatChannelActors.GetValueAs<ChattingActors>(ucc.ChannelId);

            await Task.WhenAll(
                actors.Discord.GracefulStop(TimeSpan.FromSeconds(1)),
                actors.Ottd.GracefulStop(TimeSpan.FromSeconds(1))
                );

            await
            (from chatManager in akkaService.SelectActor(MainActors.Paths.ChatChannelManager)
             from _1 in chatManager.TellExt(ucc).ToAsync()
             select _1);
        }
    }
}
