using LanguageExt;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Rcon.Messages;

namespace OpenttdDiscord.Infrastructure.Rcon.UseCases
{
    internal class RegisterRconChannelUseCase : UseCaseBase, IRegisterRconChannelUseCase
    {
        private readonly IRconChannelRepository rconChannelRepository;

        private readonly IAkkaService akkaService;

        public RegisterRconChannelUseCase(
            IRconChannelRepository rconChannelRepository,
            IAkkaService akkaService)
        {
            this.rconChannelRepository = rconChannelRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId, string prefix)
        {
            var rcon = new RconChannel(serverId, guildId, channelId, prefix);

            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _2 in rconChannelRepository.Insert(rcon)
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in actor.TellExt(new RegisterNewRconChannel(serverId, rcon)).ToAsync()
                select Unit.Default;
        }
    }
}
