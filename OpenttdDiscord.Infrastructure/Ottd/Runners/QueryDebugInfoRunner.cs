using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Runners
{
    internal class QueryDebugInfoRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        public QueryDebugInfoRunner(
            IAkkaService akkaService,
            IGetServerUseCase getServerUseCase)
            : base(akkaService)
        {
            this.getServerUseCase = getServerUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            SocketSlashCommand command,
            User user,
            ExtDictionary<string, object> options)
        {
            string serverName = options.GetValueAs<string>("server-name");
            ulong guildId = command.GuildId!.Value;
            ulong channelId = command.ChannelId!.Value;

            return
                from _1 in CheckIfHasCorrectUserLevel(
                        user,
                        UserLevel.Moderator)
                    .ToAsync()
                from server in getServerUseCase.Execute(
                    user,
                    serverName,
                    guildId)
                from _2 in AkkaService.ExecuteServerAction(
                    new QueryDebugInfo(
                        server.Id,
                        guildId,
                        channelId))
                select (ISlashCommandResponse) new TextCommandResponse("Executing");
        }
    }
}