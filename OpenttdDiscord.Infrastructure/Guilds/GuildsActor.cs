using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Guilds.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    public class GuildsActor : ReceiveActorBase
    {
        private readonly IGetAllGuildsUseCase getAllGuildsUseCase;
        private readonly Dictionary<ulong, IActorRef> guildActors = new();

        public GuildsActor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            getAllGuildsUseCase = SP.GetRequiredService<IGetAllGuildsUseCase>();
            Ready();

            Self.Tell(new InitGuildActorMessage());
        }

        public static Props Create(IServiceProvider sp) =>
            Props.Create(() => new GuildsActor(sp));

        public void Ready()
        {
            ReceiveAsync<InitGuildActorMessage>(InitGuildActorMessage);
            Receive<AddNewGuildActorMessage>(AddNewGuildActorMessage);
            Receive<ExecuteServerAction>(ExecuteServerAction);
        }

        private async Task InitGuildActorMessage(InitGuildActorMessage _)
        {
            (await getAllGuildsUseCase.Execute())
                .ThrowIfError()
                .Select(guilds => guilds.Select(g => new AddNewGuildActorMessage(g)))
                .Map(msgs => Self.TellMany(msgs));
        }

        private void AddNewGuildActorMessage(AddNewGuildActorMessage msg)
        {
            logger.LogInformation($"Creating GuildActor for {msg.GuildId}");
            IActorRef actor = Context.ActorOf(GuildActor.Create(SP, msg.GuildId), MainActors.Names.Guild(msg.GuildId));
            guildActors.Add(msg.GuildId, actor);
        }

        private void ExecuteServerAction(ExecuteServerAction msg)
        {
            if (!guildActors.TryGetValue(msg.GuildId,out var guildActor)) 
            {
                return;
            }

            guildActor.Tell(msg);
        }
    }
}
