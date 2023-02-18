using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;

namespace OpenttdDiscord.Infrastructure.Guilds
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
