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

        public ReceiveActorBase(IServiceProvider serviceProvider)
        {
            this.serviceScope = serviceProvider.CreateScope();
            this.SP = serviceScope.ServiceProvider;

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
    }
}
