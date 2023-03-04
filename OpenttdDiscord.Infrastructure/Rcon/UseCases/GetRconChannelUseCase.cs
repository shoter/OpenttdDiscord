using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Rcon.UseCases
{
    internal class GetRconChannelUseCase : UseCaseBase, IGetRconChannelUseCase
    {
        private readonly IRconChannelRepository rconChannelRepository;

        public GetRconChannelUseCase(IRconChannelRepository rconChannelRepository)
        {
            this.rconChannelRepository = rconChannelRepository;
        }

        public EitherAsync<IError, RconChannel> Execute(User user, Guid serverId, ulong guildId, ulong channelId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from option in rconChannelRepository.GetRconChannel(serverId, channelId)
                from rcon in option.ToEitherAsync((IError)new HumanReadableError("Rcon channel not found"))
                select rcon;
        }

        public EitherAsync<IError, List<RconChannel>> Execute(User user, Guid serverId, ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from rconChannels in rconChannelRepository.GetRconChannelsForTheServer(serverId)
                select rconChannels;
        }
    }
}
