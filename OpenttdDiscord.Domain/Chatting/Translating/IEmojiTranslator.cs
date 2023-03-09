using System.Text;

namespace OpenttdDiscord.Domain.Chatting.Translating
{
    internal interface IEmojiTranslator
    {
        EitherUnit FromDiscordToOttd(StringBuilder input);

        EitherUnit FromOttdToDiscord(StringBuilder input);
    }
}
