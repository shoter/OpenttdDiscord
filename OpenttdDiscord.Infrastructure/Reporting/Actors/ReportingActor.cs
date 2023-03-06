using Akka.Actor;
using LanguageExt.Pipes;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Ottd.Messages;
using OpenttdDiscord.Infrastructure.Reporting.Messages;

namespace OpenttdDiscord.Infrastructure.Reporting.Actors
{
    /// <summary>
    /// Listens to the reports of openttd players on the server.
    /// When someone reports something then it is sending message to the parent actor to execute action and collect all data along with report message.
    /// </summary>
    internal class ReportingActor : ReceiveActorBase
    {
        private readonly ReportChannel reportChannel1;
        public ReportingActor(IServiceProvider serviceProvider, ReportChannel reportChannel)
            : base(serviceProvider)
        {
            this.reportChannel1 = reportChannel;

            Ready();
            Self.Tell(new InitReportActor());
        }

        public static Props Create(IServiceProvider sp, ReportChannel reportChannel)
            => Props.Create(() => new ReportingActor(sp, reportChannel));

        public void Ready()
        {
            Receive<InitReportActor>(InitReportActor);
            Receive<AdminChatMessageEvent>(HandleChatMessage);
            ReceiveIgnore<IAdminEvent>();
        }

        private void HandleChatMessage(AdminChatMessageEvent msg)
        {
            if (msg.Player.ClientId == 1)
            {
                // we ignore server messages
                return;
            }

            if (msg.NetworkAction != NetworkAction.NETWORK_ACTION_SERVER_MESSAGE)
            {
                return;
            }

            // TODO?: make it configurable
            if(!msg.Message.StartsWith("!report"))
            {
                return;
            }

            string reportmessage = msg.Message.Split("!report").Last().Trim();
            logger.LogInformation($"Received report from {msg.Player.Name}: {reportmessage}");

            // TODO: Send action message to the parent
        }

        private void InitReportActor(InitReportActor _)
        {
            parent.Tell(new SubscribeToAdminEvents(Self));
        }

        protected override void PostStop()
        {
            base.PostStop();
            parent.Tell(new UnsubscribeFromAdminEvents(Self));
        }
    }
}
