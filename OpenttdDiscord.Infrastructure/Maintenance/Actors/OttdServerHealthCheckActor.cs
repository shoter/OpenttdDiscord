using System.Diagnostics;
using Akka.Actor;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Maintenance.Messages;

namespace OpenttdDiscord.Infrastructure.Maintenance.Actors
{
    public class OttdServerHealthCheckActor : ReceiveActorBase, IWithTimers
    {
        public ITimerScheduler Timers { get; set; } = default!;

        private readonly IAkkaService akkaService;

        private IAdminPortClient AdminPortClient { get; }

        private ulong GuildId { get; }

        private Guid ServerId { get; }


        public OttdServerHealthCheckActor(
            IAdminPortClient adminPortClient,
            ulong guildId,
            Guid serverId,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Timers.StartPeriodicTimer(
                "beep",
                new OttdServerHealthCheckBeep(),
                10.Seconds());

            this.AdminPortClient = adminPortClient;
            this.GuildId = guildId;
            this.ServerId = serverId;

            this.akkaService = SP.GetRequiredService<IAkkaService>();
        }

        public static Props Create(
            IAdminPortClient adminPortClient,
            ulong guildId,
            Guid serverId,
            IServiceProvider sp) => Props.Create(
            () => new OttdServerHealthCheckActor(
                adminPortClient,
                guildId,
                serverId,
                sp));

        public EitherAsyncUnit OttdServerHealthCheckBeep(OttdServerHealthCheckBeep _)
        {
            Stopwatch sw = new();
            sw.Start();
            uint pingValue = (uint) Random.Shared.Next();
            var msg = new AdminPingMessage(pingValue);
            CancellationTokenSource cts = new();
            var waitTask = AdminPortClient.WaitForEvent<AdminPongEvent>(
                msg,
                pong => pong.PongValue == pingValue,
                cts.Token);
            var delayTask = Task.Delay(3.Seconds());

            await Task.WhenAny(
                waitTask,
                delayTask);

            if (waitTask.IsCompletedSuccessfully == false)
            {
                OttdServerHealthCheck
                cts.Cancel();
                return;
            }

            sw.Stop();
            bool isHealthy = sw.Elapsed.TotalSeconds < 1;
            HealthStatus status = isHealthy ? HealthStatus.Healthy : HealthStatus.Degraded;

            OttdServerHealthCheck
        }

        private EitherAsyncUnit SendHealthCheck(OttdServerHealthCheck check) =>
            from selection in akkaService.SelectActor(MainActors.Paths.HealthCheck)
            from _1 in selection.TellExt(check).ToAsync()
            select Unit.Default;
    }
}