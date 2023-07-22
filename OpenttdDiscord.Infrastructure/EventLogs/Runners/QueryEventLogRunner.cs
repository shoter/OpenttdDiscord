using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.EventLogs.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.EventLogs.Runners
{
    internal class QueryEventLogRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IQueryEventLogUseCase queryServerChatUseCase;

        public QueryEventLogRunner(
            IGetServerUseCase getServerUseCase,
            IQueryEventLogUseCase queryServerChatUseCase,
            IAkkaService akkaService)
            : base(akkaService)
        {
            this.getServerUseCase = getServerUseCase;
            this.queryServerChatUseCase = queryServerChatUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(
            SocketSlashCommand command,
            User user,
            ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;
            string serverName = options.GetValueAs<string>("server-name");

            return
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