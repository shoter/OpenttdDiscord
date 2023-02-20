using Akka.Actor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Guilds;

namespace OpenttdDiscord.Discord.Services
{
    internal class AkkaStarterService : BackgroundService
    {
        private readonly ActorSystem actorSystem;

        private readonly IServiceProvider serviceProvider;

        private readonly ILogger logger;

        private readonly IAkkaService akkaService;

        public AkkaStarterService(
            ActorSystem actorSystem,
            ILogger<AkkaStarterService> logger,
            IAkkaService akkaService,
            IServiceProvider serviceProvider)        {
            this.actorSystem = actorSystem;
            this.logger = logger;
            this.akkaService = akkaService;
            this.serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting all akka actors");
            actorSystem.ActorOf(GuildsActor.Create(serviceProvider), MainActors.Names.Guilds);
            logger.LogInformation("Akka has been started!");
            akkaService.NotifyAboutAkkaStart();
            return Task.CompletedTask;
        }
    }
}
