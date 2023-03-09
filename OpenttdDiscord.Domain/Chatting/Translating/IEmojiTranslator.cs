using System.Text;

namespace OpenttdDiscord.Domain.Chatting.Translating
{
    public interface IEmojiTranslator
    {
        EitherUnit FromDiscordToOttd(StringBuilder input);

        EitherUnit FromOttdToDiscord(StringBuilder input);
    }
}
