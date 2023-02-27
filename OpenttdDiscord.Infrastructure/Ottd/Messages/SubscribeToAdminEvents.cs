using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages;

internal record SubscribeToAdminEvents(IActorRef Subscriber);
