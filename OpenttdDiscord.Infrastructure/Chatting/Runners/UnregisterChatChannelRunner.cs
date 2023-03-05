using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Chatting.Runners
{
    internal class UnregisterChatChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IUnregisterChatChannelUseCase unregisterChatChannelUseCase;

        private readonly IGetServerUseCase getServerByNameUseCase;

        public UnregisterChatChannelRunner(
            IUnregisterChatChannelUseCase unregisterChatChannelUseCase,
            IGetServerUseCase getServerByNameUseCase)
        {
            this.unregisterChatChannelUseCase = unregisterChatChannelUseCase;
            this.getServerByNameUseCase = getServerByNameUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            return
            from server in getServerByNameUseCase.Execute(user, serverName, guildId)
            from _1 in unregisterChatChannelUseCase.Execute(user, server.Id, guildId, channelId)
            select (ISlashCommandResponse)new TextCommandResponse($"{server.Name} unregistered from this chat channel");
        }
    }
}
