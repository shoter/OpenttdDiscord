using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using static LanguageExt.Prelude;

namespace OpenttdDiscord.Infrastructure.Servers
{
    public class ListOttdServersUseCase : IListOttdServersUseCase
    {
        private readonly ILogger logger;
        private readonly IOttdServerRepository ottdServerRepository;

        public ListOttdServersUseCase(
            ILogger<ListOttdServersUseCase> logger,
            IOttdServerRepository ottdServerRepository )
        {
            this.logger = logger;
            this.ottdServerRepository = ottdServerRepository;
        }

        public async Task<Either<IError, List<OttdServer>>> Execute(UserRights rights, ulong guildId)
        {
            logger.LogTrace("Executing with {0} for\n{1}", rights, guildId);
            if (rights.UserLevel != UserLevel.Admin)
            {
                return HumanReadableError.Left("Cannot execute this command as non-admin user!");
            }

            return await ottdServerRepository.GetServersForGuild(guildId);
        }
    }
}
