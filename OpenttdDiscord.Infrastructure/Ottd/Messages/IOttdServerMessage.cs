namespace OpenttdDiscord.Infrastructure.Ottd.Messages
{
    public interface IOttdServerMessage
    {
        Guid ServerId { get; }
    }
}