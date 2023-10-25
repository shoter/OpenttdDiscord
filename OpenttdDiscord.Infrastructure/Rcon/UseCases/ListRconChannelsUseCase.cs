using LanguageExt;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Rcon.UseCases
{
    internal class ListRconChannelsUseCase : UseCaseBase, IListRconChannelsUseCase
    {
        private readonly IRconChannelRepository rconChannelRepository;

        public ListRconChannelsUseCase(
            IRconChannelRepository rconChannelRepository)
        {
            this.rconChannelRepository = rconChannelRepository;
        }

        public EitherAsync<IError, List<RconChannel>> Execute(
            User user,
            Guid serverId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from rconChannels in rconChannelRepository.GetRconChannelsForTheServer(serverId)
                select rconChannels;
        }

        public EitherAsync<IError, List<RconChannel>> Execute(
            User user,
            ulong guildId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from rconChannels in rconChannelRepository.GetRconChannelsForTheGuild(guildId)
                select rconChannels;
        }
    }
}