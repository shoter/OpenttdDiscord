using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// Ignores all messages of type <typeparamref name="T"/> and does nothing with them.
        /// </summary>
        protected void ReceiveIgnore<T>() => Receive<T>(_ => { });
    }
}
