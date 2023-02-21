using Akka.Actor;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Ottd.Runners
{
    internal class QueryServerRunner : OttdSlashCommandRunnerBase
    {
        private readonly IAkkaService akkaService;
        private readonly IOttdServerRepository ottdServerRepository;

        public QueryServerRunner(
            IAkkaService akkaService,
            IOttdServerRepository ottdServerRepository)
        {
            this.akkaService = akkaService;
            this.ottdServerRepository = ottdServerRepository;
        }
        protected override async Task<Either<IError, ISlashCommandResponse>> RunInternal(SocketSlashCommand command, ExtDictionary<string, object> options)
        {
            if (!command.ChannelId.HasValue)
            {
                return new HumanReadableError("This command can be only run on channel!");
            }

            string serverName = options.GetValueAs<string>("server-name");
            ulong channelId = command.ChannelId.Value;
            return await ottdServerRepository.GetServerByName(command.GuildId!.Value, serverName)
           .MapAsync(async server =>
           {
               var action = new QueryServer(server.Id, command.GuildId!.Value, channelId);
               var guildsActor = await akkaService.SelectActor(MainActors.Paths.Guilds);
               guildsActor.Tell(action);
               return Unit.Default;
           })
           .Map(_ => new TextCommandResponse("Executing command"));
        }
    }
}
