using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage(
            "Maintainability",
            "AV1500:Member or local function contains too many statements",
            Justification = "This method mostly has simple assignments")]
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
            string selfName = GetType()
                .Name;
            logger.LogTrace(
                "({Path}|{SelfName}) received {Message}({Name}) ",
                Self.Path,
                selfName,
                message,
                message.GetType()
                    .Name);
            return base.AroundReceive(
                receive,
                message);
        }

        protected override void PostRestart(Exception reason)
        {
            this.logger.LogError($"Restarted due to {reason}");
            base.PostRestart(reason);
        }

        protected void ReceiveForward<T>(Func<IActorRef> actorToForward) => Receive<T>(
            msg => actorToForward()
                .Forward(msg));

        protected void ReceiveForward<T>(Func<IEnumerable<IActorRef>> actorsToForward) => Receive<T>(
            msg =>
            {
                foreach (var redirectActor in actorsToForward())
                {
                    redirectActor.Forward(msg);
                }
            });

        protected void ReceiveForward<T>(Func<Option<IActorRef>> actorToForward) => Receive<T>(
            msg => actorToForward()
                .IfSome(actor => actor.Forward(msg)));

        /// <summary>
        /// Ignores all messages of type <typeparamref name="T"/> and does nothing with them.
        /// </summary>
        protected void ReceiveIgnore<T>() => Receive<T>(_ => { });

        protected void ReceiveEitherAsync<T>(Func<T, EitherAsyncUnit> func) => ReceiveAsync<T>(
            async (msg) => (await func(msg)).ThrowIfError());

        protected void ReceiveEither<T>(Func<T, EitherUnit> func) => Receive<T>(
            (msg) => func(msg)
                .ThrowIfError());

        protected void ReceiveEitherRespondUnit<T>(Func<T, EitherUnit> func) => Receive<T>(
            (msg) =>
            {
                func(msg)
                    .ThrowIfError();
                Sender.Tell(Unit.Default);
            });

        protected void ReceiveEitherAsyncRespondUnit<T>(Func<T, EitherAsyncUnit> func)
        {
            ReceiveAsync<T>(
                async msg =>
                {
                    var sender = Sender;
                    (await func(msg)).ThrowIfError();
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