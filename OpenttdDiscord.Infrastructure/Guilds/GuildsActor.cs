using Akka.Actor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Guilds.Messages;

namespace OpenttdDiscord.Infrastructure.Guilds
{
    public class GuildsActor : ReceiveActorBase
    {
        private readonly IGetAllGuildsUseCase getAllGuildsUseCase;

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
        }

        private async Task InitGuildActorMessage(InitGuildActorMessage _)
        {
            var guildsResult = await getAllGuildsUseCase.Execute();

            if (guildsResult.IsLeft)
            {
                throw new Exception(guildsResult.Left().Reason);
            }

            var guilds = guildsResult.Right();
            this.Self.TellMany(guilds.Select(g => new AddNewGuildActorMessage(g)));
        }

        private void AddNewGuildActorMessage(AddNewGuildActorMessage msg)
        {
            logger.LogInformation($"Creating GuildActor for {msg.GuildId}");
            Context.ActorOf(GuildActor.Create(SP, msg.GuildId), $"Guild-{msg.GuildId}");
        }
    }
}
