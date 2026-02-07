using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace OpenttdDiscord.Domain.Chatting.Translating
{
    public class EmojiTranslator : IEmojiTranslator
    {
        public readonly Dictionary<string, string> EmojisToAscii = new Dictionary<string, string>()
        {
            { "😠", ">:(" },
            { "😛", ":P" },
            { "🙂", ":)" },
            { "😄", ":D" },
            { "😦", ":(" },
            { "🙁", ":(" },
            { "☺", ":)" },
            { "☻", ":)" },
            { "😭", ";(" },
            { "😮", ":O" },
            { "😉", ";)" },
            { "😐", ":|" },
            { "❤️", "<3" },
            { "😡", ":@" },
        };

        private readonly UnicodeCategory[] categoriesToRemove = new UnicodeCategory[]
        {
            UnicodeCategory.OtherLetter,
            UnicodeCategory.OtherSymbol,
            UnicodeCategory.OtherNotAssigned,
            UnicodeCategory.Surrogate,
            UnicodeCategory.Format,
            UnicodeCategory.NonSpacingMark,
        };

        public EitherUnit FromDiscordToOttd(StringBuilder input)
        {
            foreach (var emoji in EmojisToAscii)
            {
                input.Replace(emoji.Key, emoji.Value);
            }

            RemoveOtherEmjis(input);
            return Unit.Default;
        }

        public EitherUnit FromOttdToDiscord(StringBuilder input)
        {
            foreach (var emoji in EmojisToAscii)
            {
                input.Replace(emoji.Value, emoji.Key);
            }

            return Unit.Default;
        }

        [SuppressMessage("Maintainability",
                         "AV1530:Loop variable should not be written to in loop body",
                         Justification = "Do not know right now how to do it differently")]
        public Unit RemoveOtherEmjis(StringBuilder sb)
        {
            for (int i = 0; i < sb.Length;)
            {
                char ch = sb[i];
                var cat = char.GetUnicodeCategory(ch);

                if (categoriesToRemove.Contains(cat) ||
                    (cat == UnicodeCategory.OtherPunctuation && ch > 255))
                {
                    sb.Remove(i, 1);
                }
                else
                {
                    ++i;
                }
            }

            return Unit.Default;
        }
    }
}