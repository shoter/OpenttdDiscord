namespace OpenttdDiscord.Domain.Chatting.Translating
{
    public interface IChatTranslator
    {
        string FromDiscordToOttd(string input);

        string FromOttdToDiscord(string input);
    }
}
