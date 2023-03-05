using Akka.Actor;
using OpenttdDiscord.Domain.Chatting;

namespace OpenttdDiscord.Infrastructure.Chatting
{
    public record ChattingActors(ChatChannel channel, IActorRef Discord, IActorRef Ottd);
}
