using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;

namespace OpenttdDiscord.Infrastructure.Servers.UseCases
{
    internal class GetServerUseCase : UseCaseBase, IGetServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        public GetServerUseCase(IOttdServerRepository ottdServerRepository)
        {
            this.ottdServerRepository = ottdServerRepository;
        }

        public EitherAsync<IError, OttdServer> Execute(
            User user,
            string serverName,
            ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from server in ottdServerRepository.GetServerByName(
                    guildId,
                    serverName)
                select server;
        }

        public EitherAsync<IError, OttdServer> Execute(
            User user,
            Guid serverId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from server in ottdServerRepository.GetServer(serverId)
                select server;
        }
    }
}