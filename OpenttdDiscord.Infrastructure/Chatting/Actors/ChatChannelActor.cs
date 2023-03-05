using System.Threading.Channels;
using Akka.Actor;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Infrastructure.Chatting.Messages;
using OpenttdDiscord.Infrastructure.Discord.Actors;
using OpenttdDiscord.Infrastructure.Discord.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Actors
{
    internal class ChatChannelActor : ReceiveActorBase
    {
        private readonly ulong chatChannelId;
        private readonly IActorRef discordChannel;
        private readonly System.Collections.Generic.HashSet<IActorRef> subscribers = new();

        public ChatChannelActor(
            IServiceProvider serviceProvider,
            ulong chatChannelId)
            : base(serviceProvider)
        {
            this.chatChannelId = chatChannelId;
            discordChannel = Context.ActorOf(DiscordChannelActor.Create(SP, chatChannelId), "discordChannel");
            Ready();
        }

        public static Props Create(IServiceProvider sp, ulong chatChannelId)
            => Props.Create(() => new ChatChannelActor(sp, chatChannelId));

        private void Ready()
        {
            Receive<HandleDiscordMessage>(TellSubscribers);
            Receive<HandleOttdMessage>(TellSubscribers);
            Receive<RegisterToChatChannel>(RegisterToChatChannel);
            Receive<UnregisterFromChatChannel>(UnregisterFromChatChannel);
            Receive<QuerySubscriberCount>(_ => Sender.Tell(subscribers.Count));
        }

        private void TellSubscribers(object msg)
        {
            foreach (var s in subscribers)
            {
                if (s == Sender)
                {
                    continue;
                }

                s.Tell(msg);
            }
        }

        private void RegisterToChatChannel(RegisterToChatChannel msg)
        {
            subscribers.Add(msg.Subscriber);
            Sender.Tell(Unit.Default);
        }

        private void UnregisterFromChatChannel(UnregisterFromChatChannel msg)
        {
            subscribers.Remove(msg.Subscriber);
            Sender.Tell(Unit.Default);
        }

        protected override void PostStop()
        {
            base.PostStop();
            logger.LogInformation($"Removing Chat channel Actor for {chatChannelId}");
        }
    }
}
