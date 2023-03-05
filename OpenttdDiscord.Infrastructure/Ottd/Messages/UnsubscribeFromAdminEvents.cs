using Akka.Actor;

namespace OpenttdDiscord.Infrastructure.Ottd.Messages;

internal record UnsubscribeFromAdminEvents(IActorRef subscriber);
