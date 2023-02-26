using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Chatting.Messages;

internal record UnregisterFromChatChannel(IActorRef Subscriber);
