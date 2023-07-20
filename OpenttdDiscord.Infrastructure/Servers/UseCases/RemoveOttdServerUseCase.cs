﻿using System.Security.Cryptography;
using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Servers.Messages;

namespace OpenttdDiscord.Infrastructure.Servers.UseCases
{
    internal class RemoveOttdServerUseCase : UseCaseBase, IRemoveOttdServerUseCase
    {
        private readonly IOttdServerRepository ottdServerRepository;
        private readonly ILogger logger;

        public RemoveOttdServerUseCase(
            IOttdServerRepository ottdServerRepository,
            IAkkaService akkaService,
            ILogger<RemoveOttdServerUseCase> logger)
            : base(akkaService)
        {
            this.logger = logger;
            this.ottdServerRepository = ottdServerRepository;
        }

        public async Task<EitherUnit> Execute(
            User user,
            ulong guildId,
            string serverName)
        {
            logger.LogInformation($"Removing {serverName} for {user}");
            return await
                (from _1 in CheckIfHasCorrectUserLevel(
                            user,
                            UserLevel.Admin)
                        .ToAsync()
                    from server in ottdServerRepository.GetServerByName(
                        guildId,
                        serverName)
                    from _2 in ottdServerRepository.DeleteServer(server.Id)
                        .ToAsync()
                    from actor in AkkaService.SelectActor(MainActors.Paths.Guilds)
                    from _3 in actor.TellExt(new InformAboutServerDeletion(server))
                        .ToAsync()
                    select _3);
        }
    }
}