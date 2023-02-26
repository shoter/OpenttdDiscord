using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Chatting.Messages
{
    internal record RegisterToChatChannel(IActorRef Subscriber);
}
