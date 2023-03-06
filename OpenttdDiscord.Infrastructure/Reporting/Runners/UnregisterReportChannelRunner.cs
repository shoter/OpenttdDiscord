using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Reporting.Runners
{
    internal class UnregisterReportChannelRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerByNameUseCase;
        private readonly IUnregisterReportChannelUseCase unregisterReportChannelUseCase;

        public UnregisterReportChannelRunner(
            IGetServerUseCase getServerByNameUseCase,
            IUnregisterReportChannelUseCase unregisterReportChannelUseCase)
        {
            this.getServerByNameUseCase = getServerByNameUseCase;
            this.unregisterReportChannelUseCase = unregisterReportChannelUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            return
                from server in getServerByNameUseCase.Execute(user, serverName, guildId)
                from _1 in unregisterReportChannelUseCase.Execute(user, server.Id, guildId, channelId)
                select (ISlashCommandResponse)new TextCommandResponse($"Unregistered RCON channel for {serverName}");
        }
    }
}
