using System.Text;
using LanguageExt;

namespace OpenttdDiscord.Domain.Chatting.Translating
{
    public class EmojiTranslator : IEmojiTranslator
    {
        public Dictionary<string, string> EmojisToAscii = new Dictionary<string, string>()
        {
            { "😠", ">:(" },
            { "😛", ":P" },
            { "🙂", ":)" },
            { "😄", ":D" },
            { "😦", ":(" },
            { "😭", ";(" },
            { "😮", ":O" },
            { "😉", ";)" },
            { "😐", ":|" },
            { "❤️", "<3" },
            { "😡", ":@" },
        };

        public EitherUnit FromDiscordToOttd(StringBuilder input)
        {
            foreach(var emoji in EmojisToAscii)
            {
                input.Replace(emoji.Key, emoji.Value);
            }

            return Unit.Default;
        }

        public EitherUnit FromOttdToDiscord(StringBuilder input)
        {
            foreach (var emoji in EmojisToAscii)
            {
                input.Replace(emoji.Key, emoji.Value);
            }

            return Unit.Default;
        }
    }
}
