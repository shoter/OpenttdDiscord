using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.EventLogs.UseCases;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.EventLogs.Runners
{
    internal class QueryEventLogRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IQueryEventLogUseCase queryServerChatUseCase;

        public QueryEventLogRunner(
            IGetServerUseCase getServerUseCase,
            IQueryEventLogUseCase queryServerChatUseCase,
            IAkkaService akkaService,
            IGetRoleLevelUseCase getRoleLevelUseCase)
            : base(akkaService, getRoleLevelUseCase)
        {
            this.getServerUseCase = getServerUseCase;
            this.queryServerChatUseCase = queryServerChatUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            ISlashCommandInteraction command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;
            string serverName = options.GetValueAs<string>("server-name");

            return
                from _0 in CheckIfHasCorrectUserLevel(user, UserLevel.Moderator).ToAsync()
                from server in getServerUseCase.Execute(
                    user,
                    serverName,
                    guildId)
                from messages in queryServerChatUseCase.Execute(
                    user,
                    server.Id,
                    guildId)
                from response in ReplyWithFile(messages)
                select response;
        }

        private EitherAsync<IError, ISlashCommandResponse> ReplyWithFile(IReadOnlyList<string> text)
        {
            MemoryStream ms = new();
            using (var sw = new StreamWriter(
                       ms,
                       leaveOpen: true))
            {
                foreach (var line in text.Reverse())
                {
                    sw.WriteLine(line);
                }
            }

            return new StreamCommandResponse(
                ms,
                "chat.txt",
                dispose: true);
        }
    }
}