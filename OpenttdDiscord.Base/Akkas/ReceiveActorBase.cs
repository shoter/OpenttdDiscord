using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Base.Akkas
{
    public class ReceiveActorBase : ReceiveActor
    {
        private readonly IServiceScope serviceScope;

        protected readonly IServiceProvider SP;

        protected readonly ILogger logger;

        protected readonly IActorRef self;

        protected readonly IActorRef parent;

        protected ReceiveActorBase(IServiceProvider serviceProvider)
        {
            this.serviceScope = serviceProvider.CreateScope();
            this.SP = serviceScope.ServiceProvider;
            this.self = Self;
            this.parent = Context.Parent;

            ILoggerFactory loggerFactory = SP.GetRequiredService<ILoggerFactory>();
            var type = this.GetType();
            this.logger = loggerFactory.CreateLogger(type);

            logger.LogTrace($"Created {type.Name}");
        }

        protected override void PostStop()
        {
            base.PostStop();
            serviceScope.Dispose();
        }

        protected override bool AroundReceive(
            Receive receive,
            object message)
        {
            logger.LogTrace("{Path} received {Message}({Name}",
                Self.Path,
                message,
                message.GetType().Name);
            return base.AroundReceive(
                receive,
                message);
        }


        protected override void PostRestart(Exception reason)
        {
            this.logger.LogError($"Restarted due to {reason}");
            base.PostRestart(reason);
        }

        protected void ReceiveRedirect<T>(Func<IActorRef> redirect) => Receive<T>(
            msg => redirect()
                .Forward(msg));

        protected void ReceiveRedirect<T>(Func<IEnumerable<IActorRef>> redirects) => Receive<T>(
            msg =>
            {
                foreach (var r in redirects())
                {
                    r.Forward(msg);
                }
            });

        protected void ReceiveRedirect<T>(Func<Option<IActorRef>> redirect) => Receive<T>(
            msg => redirect()
                .IfSome(r => r.Forward(msg)));

        /// <summary>
        /// Ignores all messages of type <typeparamref name="T"/> and does nothing with them.
        /// </summary>
        protected void ReceiveIgnore<T>() => Receive<T>(_ => { });

        protected void ReceiveEitherAsync<T>(Func<T, EitherAsyncUnit> func) => ReceiveAsync<T>(
            async (t) => (await func(t)).ThrowIfError());

        protected void ReceiveEither<T>(Func<T, EitherUnit> func) => Receive<T>(
            (t) => func(t)
                .ThrowIfError());

        protected void ReceiveEitherRespondUnit<T>(Func<T, EitherUnit> func) => Receive<T>(
            (t) =>
            {
                func(t)
                    .ThrowIfError();
                Sender.Tell(Unit.Default);
            });

        protected void ReceiveEitherAsyncRespondUnit<T>(Func<T, EitherAsyncUnit> func)
        {
            ReceiveAsync<T>(
                async t =>
                {
                    var sender = Sender;
                    (await func(t)).ThrowIfError();
                    sender.Tell(Unit.Default);
                }
            );
        }

        protected void ReceiveRespondUnit<T>(Action<T> action) => Receive<T>(
            msg =>
            {
                action(msg);
                Sender.Tell(Unit.Default);
            });
    }
}