using System.Diagnostics;
using Akka.Actor;
using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Maintenance.Messages;

namespace OpenttdDiscord.Infrastructure.Maintenance.Actors
{
    public class OttdServerHealthCheckActor : ReceiveActorBase, IWithTimers
    {
        public ITimerScheduler Timers { get; set; } = default!;

        private readonly IAkkaService akkaService;

        private IAdminPortClient AdminPortClient { get; }

        private OttdServer OttdServer { get; }

        public OttdServerHealthCheckActor(
            IAdminPortClient adminPortClient,
            OttdServer server,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Timers.StartPeriodicTimer(
                "beep",
                new OttdServerHealthCheckBeep(),
                10.Seconds());

            this.AdminPortClient = adminPortClient;
            this.OttdServer = server;

            this.akkaService = SP.GetRequiredService<IAkkaService>();

            Ready();
        }

        public static Props Create(
            IAdminPortClient adminPortClient,
            OttdServer server,
            IServiceProvider sp) => Props.Create(
            () => new OttdServerHealthCheckActor(
                adminPortClient,
                server,
                sp));

        private void Ready()
        {
            ReceiveEitherAsync<OttdServerHealthCheckBeep>(OttdServerHealthCheckBeep);
        }

        public EitherAsyncUnit OttdServerHealthCheckBeep(OttdServerHealthCheckBeep _)
        {
            return
            from report in
                    SendPing()
                        .BiBind(
                            BindCorrect,
                            BindError)
                from _1 in SendHealthCheck(report)
                select Unit.Default;
        }

        private EitherAsync<IError, TimeSpan> SendPing() => TryAsync<Either<IError, TimeSpan>>(
                async () =>
                {
                    Stopwatch sw = new();
                    sw.Start();
                    uint pingValue = (uint)Random.Shared.Next();
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

                    sw.Stop();

                    if (waitTask.IsCompletedSuccessfully == false)
                    {
                        return new HumanReadableError("Timeout when doing ping");
                    }

                    return sw.Elapsed;
                })
            .ToEitherAsyncErrorFlat();

        private EitherAsync<IError, OttdServerHealthCheck> BindError(IError _) => new OttdServerHealthCheck(
            DateTimeOffset.Now,
            OttdServer,
            TimeSpan.Zero,
            HealthStatus.Unhealthy
        );

        private EitherAsync<IError, OttdServerHealthCheck> BindCorrect(TimeSpan span) => new OttdServerHealthCheck(
            DateTimeOffset.Now,
            OttdServer,
            span,
            (span < 1.Seconds().ToTimeSpan()) ? HealthStatus.Healthy : HealthStatus.Degraded
        );
        private EitherAsyncUnit SendHealthCheck(OttdServerHealthCheck check) =>
            from selection in akkaService.SelectActor(MainActors.Paths.HealthCheck)
            from _1 in selection.TellExt(check).ToAsync()
            select Unit.Default;
    }
}