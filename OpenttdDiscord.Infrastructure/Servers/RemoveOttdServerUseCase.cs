using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using System.Security.Cryptography;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class RemoveOttdServerUseCase : UseCaseBase, IRemoveOttdServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;
        private readonly ILogger logger;
        private readonly IAkkaService akkaService;

        public RemoveOttdServerUseCase(
            IOttdServerRepository ottdServerRepository,
            IAkkaService akkaService,
            ILogger<RemoveOttdServerUseCase> logger)
        {
            this.logger = logger;
            this.akkaService = akkaService;
            this.ottdServerRepository = ottdServerRepository;
        }

        public async Task<EitherUnit> Execute(User user, ulong guildId, string serverName)
        {
            this.logger.LogInformation($"Removing {serverName} for {user}");
            return await
            (from _1 in CheckIfHasCorrectUserLEvel(user, UserLevel.Admin).ToAsync()
             from server in ottdServerRepository.GetServerByName(guildId, serverName).ToAsync()
             from _2 in ottdServerRepository.DeleteServer(server.Id).ToAsync()
             from _3 in InformGuildsActor(server).ToAsync()
             select _1).ToEither();

            //return CheckIfHasCorrectUserLEvel(user, UserLevel.Admin)
            //    .BindAsync(_ => ottdServerRepository.GetServerByName(guildId, serverName))
            //    .BindAsync((OttdServer server) => ottdServerRepository.DeleteServer(server.Id));
        }

        private async Task<EitherUnit> InformGuildsActor(OttdServer server)
        {
            ActorSelection selection = await akkaService.SelectActor(MainActors.Paths.Guilds);
            selection.Tell(new InformAboutServerDeletion(server));
            return Unit.Default;
        }
    }
}
