using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Rcon.Runners
{
    internal class RegisterRconChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IRegisterRconChannelUseCase registerRconChannelUseCase;

        private readonly IGetServerByNameUseCase getServerByNameUseCase;

        public RegisterRconChannelRunner(
            IRegisterRconChannelUseCase registerRconChannelUseCase,
            IGetServerByNameUseCase getServerByNameUseCase)
        {
            this.registerRconChannelUseCase = registerRconChannelUseCase;
            this.getServerByNameUseCase = getServerByNameUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            string serverName = options.GetValueAs<string>("server-name");
            string prefix = options.GetValueAs<string>("prefix");

            return
                from server in getServerByNameUseCase.Execute(user, serverName, guildId)
                from _1 in registerRconChannelUseCase.Execute(user, server.Id, guildId, channelId, prefix)
                select (ISlashCommandResponse)new TextCommandResponse("Rcon channel registered!");
        }
    }
}
