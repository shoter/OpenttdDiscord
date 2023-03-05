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
    internal class QueryServerChatRunner : OttdSlashCommandRunnerBase
    {
        private readonly IGetServerUseCase getServerUseCase;

        private readonly IQueryServerChatUseCase queryServerChatUseCase;

        public QueryServerChatRunner(IGetServerUseCase getServerUseCase, IQueryServerChatUseCase queryServerChatUseCase)
        {
            this.getServerUseCase = getServerUseCase;
            this.queryServerChatUseCase = queryServerChatUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;
            string serverName = options.GetValueAs<string>("server-name");

            return
                from server in getServerUseCase.Execute(user, serverName, guildId)
                from messages in queryServerChatUseCase.Execute(user, server.Id, guildId)
                from response in ReplyWithFile(messages)
                select response;
        }

        private EitherAsync<IError, ISlashCommandResponse> ReplyWithFile(IReadOnlyList<string> text)
        {
            MemoryStream ms = new();
            using (var sw = new StreamWriter(ms, leaveOpen: true))
            {
                foreach (var line in text)
                {
                    sw.WriteLine(text);
                }
            }

            return new StreamCommandResponse(ms, "chat.txt", dispose: true);
        }
    }
}
