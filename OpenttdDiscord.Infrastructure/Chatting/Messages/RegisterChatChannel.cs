using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Infrastructure.Ottd.Messages;

namespace OpenttdDiscord.Infrastructure.Chatting.Messages
{
    public record RegisterChatChannel(ChatChannel chatChannel) : IOttdServerMessage
    {
        public Guid ServerId => chatChannel.ServerId;
    }
}
