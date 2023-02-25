using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Chatting.Messages
{
    public record RegisterChatChannel(OttdServer server, ChatChannel chatChannel);
}
