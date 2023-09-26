using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Runners
{
    internal class UnregisterRconChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerByNameUseCase;
        private readonly IUnregisterRconChannelUseCase unregisterRconChannelUseCase;

        public UnregisterRconChannelRunner(
            IGetServerUseCase getServerByNameUseCase,
            IUnregisterRconChannelUseCase unregisterRconChannelUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
        : base(akkaService, getRoleLevelUseCase)
        {
            this.getServerByNameUseCase = getServerByNameUseCase;
            this.unregisterRconChannelUseCase = unregisterRconChannelUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(ISlashCommandInteraction command, User user, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Admin).ToAsync()
                from server in getServerByNameUseCase.Execute(user, serverName, guildId)
                from _1 in unregisterRconChannelUseCase.Execute(user, server.Id, guildId, channelId)
                select (ISlashCommandResponse)new TextCommandResponse($"Unregistered RCON channel for {serverName}");
        }
    }
}
