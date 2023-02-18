using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Validation;
using OpenttdDiscord.Validation.Ottd;

namespace OpenttdDiscord.Infrastructure.Servers
{
    public class RegisterOttdServerUseCase : IRegisterOttdServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;
        private readonly ILogger logger;
        private readonly OttdValidator<OttdServerValidator, OttdServer> validator = new(new());

        public RegisterOttdServerUseCase(
            ILogger<RegisterOttdServerUseCase> logger,
            IOttdServerRepository ottdServerRepository
            )
        {
            this.ottdServerRepository = ottdServerRepository;
            this.logger = logger;
        }

        public async Task<EitherUnit> Execute(UserRights userRights, OttdServer server)
        {
            return await validator.Validate(server)
                .Bind(_ =>
                {
                    logger.LogTrace("Executing with {0} for\n{1}", userRights, server);
                    if (userRights.UserLevel != UserLevel.Admin)
                    {
                        return HumanReadableError.EitherUnit("Cannot execute this command as non-admin user!");
                    }
                    return Unit.Default;
                })
                .BindAsync<IError, Unit, Unit>(async _ =>
                {
                    var existing = await ottdServerRepository.GetServerByName(server.Name);
                    if (existing.IsRight)
                    {
                        return new HumanReadableError("Server with this name already exists!");
                    }

                    return Unit.Default;
                })
                .BindAsync(_ => ottdServerRepository.InsertServer(server));
        }
    }
}
