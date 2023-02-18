using Microsoft.Extensions.Logging;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Servers
{
    internal class RemoveOttdServerUseCase : UseCaseBase, IRemoveOttdServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;
        private readonly ILogger logger;

        public RemoveOttdServerUseCase(IOttdServerRepository ottdServerRepository, ILogger<RemoveOttdServerUseCase> logger)
        {
            this.logger = logger;
            this.ottdServerRepository = ottdServerRepository;
        }

        public Task<EitherUnit> Execute(User user, string serverName)
        {
            this.logger.LogInformation($"Removing {serverName} for {user}");

            return CheckIfHasCorrectUserLEvel(user, UserLevel.Admin)
                .BindAsync(_ => ottdServerRepository.GetServerByName(serverName))
                .BindAsync((OttdServer server) => ottdServerRepository.DeleteServer(server.Id));
        }
    }
}
