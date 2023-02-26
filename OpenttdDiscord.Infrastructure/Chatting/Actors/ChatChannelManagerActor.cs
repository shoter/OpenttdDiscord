using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Infrastructure.Chatting.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    public class ChatChannelManagerActor : ReceiveActorBase
    {
        public ExtDictionary<ulong, IActorRef> Channels { get; } = new();
        private ChatChannelManagerActor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Ready();
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new ChatChannelManagerActor(sp));

        private void Ready()
        {
            Receive<GetCreateChatChannel>(GetCreateChatChannel);
        }

        private void GetCreateChatChannel(GetCreateChatChannel msg)
        {
            Option<IActorRef> channel = Channels.MaybeGetValue(msg.ChannelId);
            IActorRef channelActor = channel.IfNone(() => CreateChatChannel(msg.ChannelId));
            Sender.Tell(channelActor);
        }

        private IActorRef CreateChatChannel(ulong channelId)
        {
            return Context.ActorOf(ChatChannelActor.Create(SP, channelId), channelId.ToString());
        }
    }
}
