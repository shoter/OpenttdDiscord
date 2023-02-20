using Akka.Actor;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Runners
{
    internal class QueryServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IAkkaService akkaService;

        public QueryServerRunner(IAkkaService akkaService)
        {
            this.akkaService = akkaService;
        }
        protected override async Task<Either<IError, ISlashCommandResponse>> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            if (!command.ChannelId.HasValue)
            {
                return new HumanReadableError("This command can be only run on channel!");
            }

            string serverName = options.GetValueAs<string>("server-name");
            ulong channelId = command.ChannelId.Value;

            var message = new QueryServerMessage(channelId, serverName);
            var guildsActor = await akkaService.SelectActor(MainActors.Paths.Guilds);
            guildsActor.Tell(message);
            return new TextCommandResponse("Executing command");
        }
    }
}
