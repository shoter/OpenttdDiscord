using Akka.Actor;
using LanguageExt.UnitsOfMeasure;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using Serilog;

namespace OpenttdDiscord.Infrastructure.Ottd.Actors
{
    internal class GuildServerActor : ReceiveActorBase
    {
        private readonly OttdServer server;
        private readonly AdminPortClient client;

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
        }

        public static Props Create(IServiceProvider sp, OttdServer server)
            => Props.Create(() => new GuildServerActor(sp, server));

        protected override void PostStop()
        {
            base.PostStop();
        }

        private async Task InitGuildServerActorMessage(InitGuildServerActorMessage _)
        {
            logger.LogInformation($"Connecting to {server.Name} on {server.Ip}:{server.AdminPort}");
            await client.Connect();
        }
    }
}
