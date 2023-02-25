using Akka.Actor;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Servers.Messages;
using OpenttdDiscord.Validation;
using OpenttdDiscord.Validation.Ottd;

namespace OpenttdDiscord.Infrastructure.Servers.UseCases
{
    internal class RegisterOttdServerUseCase : UseCaseBase, IRegisterOttdServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;
        private readonly ILogger logger;
        private readonly OttdValidator<OttdServerValidator, OttdServer> validator = new(new());
        private readonly IAkkaService akkaService;

        public RegisterOttdServerUseCase(
            ILogger<RegisterOttdServerUseCase> logger,
            IAkkaService akkaService,
            IOttdServerRepository ottdServerRepository
            )
        {
            this.ottdServerRepository = ottdServerRepository;
            this.akkaService = akkaService;
            this.logger = logger;
        }

        public EitherAsyncUnit Execute(User userRights, OttdServer server)
        {
            logger.LogTrace("Executing with {0} for\n{1}", userRights, server);

            return
                from _1 in validator.Validate(server).ToAsync()
                from _2 in CheckIfSerwerExists(server.GuildId, server.Name)
                from _3 in ottdServerRepository.InsertServer(server).ToAsync()
                from _4 in InformActorAboutNewServer(server)
                select _4;
        }

        private EitherAsyncUnit CheckIfSerwerExists(ulong guildId, string serverName)
            => TryAsync<EitherUnit>(async () =>
            {
                var existing = await ottdServerRepository.GetServerByName(guildId, serverName);
                if (existing.IsRight)
                {
                    return new HumanReadableError("Server with this name already exists!");
                }

                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        private EitherAsyncUnit InformActorAboutNewServer(OttdServer server)
            => TryAsync(async () =>
            {
                ActorSelection selection = await akkaService.SelectActor(MainActors.Paths.Guilds);
                selection.Tell(new InformAboutServerRegistration(server));
                return Unit.Default;
            }).ToEitherAsyncError();
    }
}
