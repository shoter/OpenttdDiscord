using Akka.Actor;
using LanguageExt;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Actors
{
    public class WelcomeActor : ReceiveActorBase
    {
        private readonly IAdminPortClient client;
        private string[] messagesToSend;

        public WelcomeActor(
            IServiceProvider serviceProvider,
            IAdminPortClient client,
            string welcomeMessageContent)
            : base(serviceProvider)
        {
            this.client = client;
            this.messagesToSend = CreateMessageToSent(welcomeMessageContent);
            Ready();
        }

        private string[] CreateMessageToSent(string arg)
        {
            return arg
                .Replace("\r", string.Empty)
                .Split('\n');
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
            ReceiveRespondUnit<UpdateWelcomeMessage>(msg => this.messagesToSend = CreateMessageToSent(msg.Content));
            ReceiveIgnore<IAdminEvent>();
        }

        private void OnAdminClientJoinEvent(AdminClientJoinEvent arg)
        {
            foreach (var msg in messagesToSend)
            {
                client.SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        arg.Player.ClientId,
                        msg));
            }

            Sender.Tell(Unit.Default);
        }
    }
}