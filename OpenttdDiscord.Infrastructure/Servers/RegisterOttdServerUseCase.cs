using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Servers
{
    public class RegisterOttdServerUseCase : IRegisterOttdServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;
        private readonly ILogger logger;

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
            logger.LogTrace("Executing with {0} for\n{1}", userRights, server);
            if(userRights.UserLevel != UserLevel.Admin)
            {
                return HumanReadableError.EitherUnit("Cannot execute this command as non-admin user!");
            }

            var existing = await ottdServerRepository.GetServerByName(server.Name);

            if(existing.IsRight)
            {
                return new HumanReadableError("Server with this name already exists!");
            }

            return await ottdServerRepository.InsertServer(server);
        }
    }
}
