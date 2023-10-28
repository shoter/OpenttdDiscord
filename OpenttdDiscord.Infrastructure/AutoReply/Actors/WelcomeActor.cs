using Akka.Actor;
using LanguageExt;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReply.Actors
{
    public class WelcomeActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private readonly string welcomeMessageContent;

        public WelcomeActor(
            IServiceProvider serviceProvider,
            IAdminPortClient client,
            string welcomeMessageContent)
            : base(serviceProvider)
        {
            this.client = client;
            this.welcomeMessageContent = welcomeMessageContent;
            Ready();
        }

        public static Props Create(
            IServiceProvider serviceProvider,
            IAdminPortClient client,
            string welcomeMessageContent) => Props.Create(
            () => new WelcomeActor(
                serviceProvider,
                client,
                welcomeMessageContent));

        private void Ready()
        {
            Receive<AdminClientJoinEvent>(OnAdminClientJoinEvent);
            ReceiveIgnore<IAdminEvent>();
        }

        private void OnAdminClientJoinEvent(AdminClientJoinEvent arg)
        {
            client.SendMessage(
                new AdminChatMessage(
                    NetworkAction.NETWORK_ACTION_CHAT,
                    ChatDestination.DESTTYPE_CLIENT,
                    arg.Player.ClientId,
                    welcomeMessageContent));

            Sender.Tell(Unit.Default);
        }
    }
}