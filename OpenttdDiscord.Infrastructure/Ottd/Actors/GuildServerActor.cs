using Akka.Actor;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Servers;
using OpenttdDiscord.Infrastructure.Statuses.Messages;
using Serilog;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal class GuildServerActor : ReceiveActorBase, IWithTimers
    {
        private readonly OttdServer server;
        private readonly AdminPortClient client;

        public ITimerScheduler Timers { get; set; } = default!;

        public GuildServerActor(IServiceProvider serviceProvider, OttdServer server) : base(serviceProvider)
        {
            this.server = server;
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

        }
    }
}
