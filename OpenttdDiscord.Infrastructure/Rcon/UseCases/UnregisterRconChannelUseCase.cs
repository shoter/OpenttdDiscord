﻿using LanguageExt;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Rcon.Messages;

namespace OpenttdDiscord.Infrastructure.Rcon.UseCases
{
    internal class UnregisterRconChannelUseCase : UseCaseBase, IUnregisterRconChannelUseCase
    {
        private readonly IRconChannelRepository rconChannelRepository;

        private readonly IAkkaService akkaService;

        public UnregisterRconChannelUseCase(IRconChannelRepository rconChannelRepository, IAkkaService akkaService)
        {
            this.rconChannelRepository = rconChannelRepository;
            this.akkaService = akkaService;
        }

        public EitherAsyncUnit Execute(User user, Guid serverId, ulong guildId, ulong channelId)
        {
            return
                from _1 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from _2 in rconChannelRepository.Delete(serverId, channelId)
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from _3 in actor.TellExt(new UnregisterRconChannel(serverId, guildId, channelId)).ToAsync()
                select Unit.Default;
        }
    }
}