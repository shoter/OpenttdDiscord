using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies.UseCases;
using OpenttdDiscord.Infrastructure.AutoReply.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReply.Actors
{
    public class AutoReplyActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private readonly IGetWelcomeMessageUseCase getWelcomeMessageUseCase;
        private Option<IActorRef> welcomeActor = Option<IActorRef>.None;

        public AutoReplyActor(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.client = client;
            this.getWelcomeMessageUseCase = SP.GetRequiredService<IGetWelcomeMessageUseCase>();

            var welcomeMessage = getWelcomeMessageUseCase.Execute(
                    guildId,
                    serverId)
                .AsTask()
                .Result
                .Right();
            
            welcomeMessage.IfSome(
                message =>
                
                )
            
            
            
            
            

            Ready();
        }
        
        public static Props Create(
            IServiceProvider serviceProvider,
            ulong guildId,
            Guid serverId,
            IAdminPortClient client) => Props.Create(
            () => new AutoReplyActor(
                serviceProvider,
                guildId,
                serverId,
                client));

        private void Ready()
        {
            Receive<UpdateWelcomeMessage>(UpsertWelcomeMessage);
            Receive<IAdminEvent>(OnAdminEvent);
        }

        private void OnAdminEvent(IAdminEvent msg)
        {
            welcomeActor.TellExt(msg);
            Sender.Tell(Unit.Default);
        }

        private void UpsertWelcomeMessage(UpdateWelcomeMessage msg)
        {
            if (welcomeActor.IsSome)
            {
                welcomeActor.ForwardExt(msg);
                Sender.Tell(Unit.Default);
                return;
            }

            IActorRef actor = Context.ActorOf(
                WelcomeActor.Create(
                    SP,
                    client,
                    msg.Content));

            welcomeActor = Some(actor);
            Sender.Tell(Unit.Default);
        }
    }
}