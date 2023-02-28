using Akka.Actor;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Chatting.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    public class ChatChannelManagerActor : ReceiveActorBase
    {
        public ExtDictionary<ulong, IActorRef> Channels { get; } = new();
        public ChatChannelManagerActor(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Ready();
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new ChatChannelManagerActor(sp));

        private void Ready()
        {
            Receive<GetCreateChatChannel>(GetCreateChatChannel);
            Receive<UnregisterChatChannel>(UnregisterChatChannel);
        }

        private void GetCreateChatChannel(GetCreateChatChannel msg)
        {
            Option<IActorRef> channel = Channels.MaybeGetValue(msg.ChannelId);
            IActorRef channelActor = channel.IfNone(() => CreateChatChannel(msg.ChannelId));
            Sender.Tell(channelActor);
        }

        private IActorRef CreateChatChannel(ulong channelId)
        {
            IActorRef channel = Context.ActorOf(ChatChannelActor.Create(SP, channelId), channelId.ToString());
            Channels.Add(channelId, channel);
            return channel;
        }

        private async Task UnregisterChatChannel(UnregisterChatChannel ucc)
        {
            EitherAsyncUnit RemoveIfCountEqualsZero(int count, IActorRef channel)
                => TryAsync(async () =>
                {
                    if (count == 0)
                    {
                        return Unit.Default;
                    }

                    await channel.GracefulStop(TimeSpan.FromSeconds(1));
                    return Unit.Default;
                }).ToEitherAsyncError();

            (await(
            from channel in Channels.MaybeGetValue(ucc.ChannelId).ToEitherAsync((IError) new HumanReadableError("Channel not found"))
             from count in channel.TryAsk<int>(new QuerySubscriberCount())
             from _1 in RemoveIfCountEqualsZero(count, channel)
             select _1
             )).LeftLogError(logger);
        }
    }
}
