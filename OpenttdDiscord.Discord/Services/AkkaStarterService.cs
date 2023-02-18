using Akka.Actor;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Infrastructure.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Discord.Services
{
    internal class AkkaStarterService : BackgroundService
    {
        private readonly ActorSystem actorSystem;

        private readonly IServiceProvider serviceProvider;

        private readonly ILogger logger;

        public AkkaStarterService(
            ActorSystem actorSystem,
            ILogger<AkkaStarterService> logger,
            IServiceProvider serviceProvider)        {
            this.actorSystem = actorSystem;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting all akka actors");
            actorSystem.ActorOf(GuildsActor.Create(serviceProvider), "Guilds");
            logger.LogInformation("Akka has been started!");
            return Task.CompletedTask;
        }
    }
}
