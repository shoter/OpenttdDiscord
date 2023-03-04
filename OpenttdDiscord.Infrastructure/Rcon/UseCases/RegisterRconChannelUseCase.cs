using LanguageExt;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;

namespace OpenttdDiscord.Infrastructure.Rcon.UseCases
{
    internal class RegisterRconChannelUseCase : UseCaseBase, IRegisterRconChannelUseCase
    {
        private readonly IRconChannelRepository rconChannelRepository;

        public RegisterRconChannelUseCase(IRconChannelRepository rconChannelRepository)
        {
            this.rconChannelRepository = rconChannelRepository;
        }

        public EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId, string prefix)
        {
            var rcon = new RconChannel(serverId, guildId, channelId, prefix);

            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _2 in rconChannelRepository.Insert(rcon)
                select Unit.Default;
        }
    }
}
