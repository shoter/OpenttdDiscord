using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Infrastructure.Chatting.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    public class ChatChannelManagerActor : ReceiveActorBase
    {
        public ExtDictionary<ulong, IActorRef> Channels { get; } = new();

        public ChatChannelManagerActor(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Ready();
        }

        public static Props Create(IServiceProvider sp)
            => Props.Create(() => new ChatChannelManagerActor(sp));

        private void Ready()
        {
            Receive<GetCreateChatChannel>(GetCreateChatChannel);
            ReceiveAsync<UnregisterChatChannel>(UnregisterChatChannel);
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
            EitherAsync<IError, bool> RemoveIfCountEqualsZero(int count, IActorRef channel)
                => TryAsync(async () =>
                {
                    if (count == 0)
                    {
                        return false;
                    }

                    await channel.GracefulStop(TimeSpan.FromSeconds(1));
                    return true;
                }).ToEitherAsyncError();

            (await (
            from channel in Channels.MaybeGetValue(ucc.ChannelId).ToEitherAsync((IError)new HumanReadableError("Channel not found"))
            from count in channel.TryAsk<int>(new QuerySubscriberCount(), TimeSpan.FromSeconds(2))
            from isRemoved in RemoveIfCountEqualsZero(count, channel)
            select isRemoved
             ))
             .LeftLogError(logger)
             .Right(isRemoved =>
             {
                 if (isRemoved)
                 {
                     logger.LogInformation($"Removed chat channel {ucc.ChannelId}");
                 }
             });
        }
    }
}
