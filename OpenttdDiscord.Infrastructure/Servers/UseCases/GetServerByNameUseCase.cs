using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;

namespace OpenttdDiscord.Infrastructure.Servers.UseCases
{
    internal class GetServerByNameUseCase : UseCaseBase, IGetServerByNameUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        public GetServerByNameUseCase(IOttdServerRepository ottdServerRepository)
        {
            this.ottdServerRepository = ottdServerRepository;
        }

        public EitherAsync<IError, OttdServer> Execute(User user, string serverName, ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from server in ottdServerRepository.GetServerByName(guildId, serverName)
                select server;
        }
    }
}
