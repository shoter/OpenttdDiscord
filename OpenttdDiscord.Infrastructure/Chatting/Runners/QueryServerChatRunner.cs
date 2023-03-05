using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;

namespace OpenttdDiscord.Infrastructure.Chatting.Runners
{
    internal class QueryServerChatRunner : OttdSlashCommandRunnerBase
    {
        private readonly IAkkaService akkaService;

        private readonly DiscordSocketClient discord;

        private readonly IGetServerUseCase getServerUseCase;

        public QueryServerChatRunner(IAkkaService akkaService, IGetServerUseCase getServerUseCase, DiscordSocketClient discord)
        {
            this.akkaService = akkaService;
            this.getServerUseCase = getServerUseCase;
            this.discord = discord;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;
            string serverName = options.GetValueAs<string>("server-name");

            return
                from actor in akkaService.SelectActor(MainActors.Paths.Guilds)
                from x in actor.TryAsk<RetrievedChatMessages>(new RetrieveChatMessages())

        }

        private EitherAsync<IError, ISlashCommandResponse> ReplyWithFile(List<string> text)
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
