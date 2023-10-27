using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Rcon.UseCases
{
    internal class GetRconChannelUseCase : UseCaseBase, IGetRconChannelUseCase
    {
        private readonly IRconChannelRepository rconChannelRepository;

        public GetRconChannelUseCase(
            IRconChannelRepository rconChannelRepository)
        {
            this.rconChannelRepository = rconChannelRepository;
        }

        public EitherAsync<IError, Option<RconChannel>> Execute(
            User user,
            Guid serverId,
            ulong channelId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from option in rconChannelRepository.GetRconChannel(
                    serverId,
                    channelId)
                select option;
        }
    }
}