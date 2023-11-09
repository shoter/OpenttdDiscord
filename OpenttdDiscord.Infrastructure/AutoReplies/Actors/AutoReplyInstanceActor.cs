using Akka.Actor;
using LanguageExt.Pipes;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Base.Openttd;
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
            ReceiveEitherAsyncRespondUnit<AdminChatMessageEvent>(OnAdminChatMessageEvent);
            Receive<UpdateAutoReply>(UpdateAutoReply);
        }

        private void UpdateAutoReply(UpdateAutoReply msg)
        {
            this.autoReply = msg.AutoReply;
            this.slicedMessage = CreateSlicedMessage(msg.AutoReply);
        }

        private EitherAsyncUnit OnAdminChatMessageEvent(AdminChatMessageEvent msg)
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
                (from _0 in ExecuteAdditionalAction(msg)
                    from _1 in SendMessageToClient(msg.Player.ClientId)
                    select Unit.Default).IfLeft(
                    left =>
                    {
                        if (left is HumanReadableError hre)
                        {
                            adminPortClient.SendMessage(
                                new AdminChatMessage(
                                    NetworkAction.NETWORK_ACTION_CHAT,
                                    ChatDestination.DESTTYPE_CLIENT,
                                    msg.Player.ClientId,
                                    hre.Reason));
                        }
                        else
                        {
                            left.LogError(logger);
                        }

                        return Unit.Default;
                    });
        }

        private EitherAsyncUnit SendMessageToClient(uint clientId)
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

        private EitherAsyncUnit ExecuteAdditionalAction(AdminChatMessageEvent msg)
        {
            switch (autoReply.AdditionalAction)
            {
                case AutoReplyAction.ResetCompany:
                {
                    // max value of byte is a spectator
                    if (msg.Player.PlayingAs == byte.MaxValue)
                    {
                        return new HumanReadableError("You cannot reset company while being a spectator");
                    }

                    return from status in adminPortClient.QueryServerStatusExt()
                        from _0 in ErrorWhenMorePlayersInCompany(
                            status,
                            msg.Player.PlayingAs)
                        from _1 in ResetCompany(msg)
                        select Unit.Default;
                }
            }

            return Unit.Default;
        }

        private EitherAsyncUnit ErrorWhenMorePlayersInCompany(
            ServerStatus status,
            byte playingAs)
        {
            if (status.Players.Values
                    .Count(x => x.PlayingAs == playingAs) > 1)
            {
                return new HumanReadableError(
                    "There is more than 1 player in company. It is impossible to reset company");
            }

            return Unit.Default;
        }

        private EitherAsyncUnit ResetCompany(AdminChatMessageEvent msg)
        {
            string command = $"move {msg.Player.ClientId} {byte.MaxValue}";
            adminPortClient.SendMessage(new AdminRconMessage(command));
            command = $"reset_company {msg.Player.PlayingAs + 1}";
            adminPortClient.SendMessage(new AdminRconMessage(command));
            return Unit.Default;
        }
    }
}