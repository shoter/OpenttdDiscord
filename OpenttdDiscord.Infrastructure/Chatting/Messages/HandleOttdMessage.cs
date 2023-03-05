using OpenttdDiscord.Domain.Servers;

namespace OpenttdDiscord.Infrastructure.Chatting.Messages
{
    internal record HandleOttdMessage(OttdServer Server, string Username, string Message);
}
