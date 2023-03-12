using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Guilds.UseCases;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Guilds.UseCases
{
    internal class GetAllGuildsUseCase : IGetAllGuildsUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;

        public GetAllGuildsUseCase(IOttdServerRepository ottdServerRepository)
        {
            this.ottdServerRepository = ottdServerRepository;
        }

        public Task<Either<IError, List<ulong>>> Execute()
            => ottdServerRepository.GetAllGuilds();
    }
}
