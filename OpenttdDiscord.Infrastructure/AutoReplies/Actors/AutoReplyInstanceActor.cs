using Akka.Actor;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Domain.AutoReplies;
using OpenttdDiscord.Infrastructure.AutoReplies.Messages;

namespace OpenttdDiscord.Infrastructure.AutoReplies.Actors
{
    public class AutoReplyInstanceActor : ReceiveActorBase
    {
        private readonly IAdminPortClient adminPortClient;
        private string[] slicedMessage;
        private AutoReply autoReply;

        public AutoReplyInstanceActor(
            IServiceProvider serviceProvider,
            AutoReply autoReply,
            IAdminPortClient client)
            : base(serviceProvider)
        {
            this.autoReply = autoReply;
            this.adminPortClient = client;
            this.slicedMessage = CreateSlicedMessage(autoReply);
            Ready();
        }

        private string[] CreateSlicedMessage(AutoReply autoReply)
        {
             return autoReply.ResponseMessage
                .Replace(
                    "\r",
                    string.Empty)
                .Split('\n')
                .SelectMany(x => x.Chunk(500))
                .Select(x => new string(x))
                .ToArray();
        }

        public static Props Create(
            IServiceProvider sp,
            AutoReply ar,
            IAdminPortClient client) => Props.Create(
            () => new AutoReplyInstanceActor(
                sp,
                ar,
                client));

        private void Ready()
        {
            ReceiveEitherRespondUnit<AdminChatMessageEvent>(OnAdminChatMessageEvent);
            Receive<UpdateAutoReply>(UpdateAutoReply);
        }

        private void UpdateAutoReply(UpdateAutoReply msg)
        {
            this.autoReply = msg.AutoReply;
            this.slicedMessage = CreateSlicedMessage(msg.AutoReply);
        }

        private EitherUnit OnAdminChatMessageEvent(AdminChatMessageEvent msg)
        {
            if (msg.Player.ClientId == 1)
            {
                return Unit.Default;
            }

            if (msg.Message != autoReply.TriggerMessage)
            {
                return Unit.Default;
            }

            return
                from _1 in SendMessageToClient(msg.Player.ClientId)
                from _2 in ExecuteAdditionalAction(msg)
                select Unit.Default;
        }

        private EitherUnit SendMessageToClient(uint clientId)
        {
            foreach (var text in slicedMessage)
            {
                adminPortClient.SendMessage(
                    new AdminChatMessage(
                        NetworkAction.NETWORK_ACTION_CHAT,
                        ChatDestination.DESTTYPE_CLIENT,
                        clientId,
                        text));
            }

            return Unit.Default;
        }

        private EitherUnit ExecuteAdditionalAction(AdminChatMessageEvent msg)
        {
            switch (autoReply.AdditionalAction)
            {
                case AutoReplyAction.ResetCompany:
                {
                    string command = $"reset_company {msg.Player.PlayingAs}";
                    adminPortClient.SendMessage(new AdminRconMessage(command));
                    break;
                }
            }

            return Unit.Default;
        }
    }
}