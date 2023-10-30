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
            string serverName,
            ulong guildId)
        {
            return
                from server in ottdServerRepository.GetServerByName(
                    guildId,
                    serverName)
                select server;
        }

        public EitherAsync<IError, OttdServer> Execute(
            Guid serverId)
        {
            return
                from server in ottdServerRepository.GetServer(serverId)
                select server;
        }
    }
}