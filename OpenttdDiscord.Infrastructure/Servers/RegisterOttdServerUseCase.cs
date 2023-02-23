using Akka.Actor;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Validation;
using OpenttdDiscord.Validation.Ottd;

namespace OpenttdDiscord.Infrastructure.Servers
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

        public async Task<EitherUnit> Execute(User userRights, OttdServer server)
        {
            logger.LogTrace("Executing with {0} for\n{1}", userRights, server);

            return await validator.Validate(server)
                .Bind(_ => this.CheckIfHasCorrectUserLEvel(userRights, UserLevel.Admin))
                .BindAsync<IError, Unit, Unit>(async _ =>
                {
                    var existing = await ottdServerRepository.GetServerByName(server.GuildId, server.Name);
                    if (existing.IsRight)
                    {
                        return new HumanReadableError("Server with this name already exists!");
                    }

                    return Unit.Default;
                })
                .BindAsync(_ => ottdServerRepository.InsertServer(server))
                .MapAsync(async _ =>
                {
                    ActorSelection selection = await akkaService.SelectActor(MainActors.Paths.Guilds);
                    selection.Tell(new InformAboutServerRegistration(server));
                    return Unit.Default;
                });
        }
    }
}
