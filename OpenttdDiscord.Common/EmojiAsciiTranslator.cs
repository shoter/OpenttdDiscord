using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common
{
    public class EmojiAsciiTranslator
    {
        public Dictionary<string, string> EmojisToAscii = new Dictionary<string, string>()
        {
            { "😛", ":P" },
            { "🙂", ":)" },
            { "😄", ":D" },
            { "😦", ":(" },
            { "😭", ";(" },
            { "😮", ":O" },
            { "😉", ";)" },
            { "😐", ":|" },
            { "😠", ">:(" },
            { "❤️", "<3" },
            { "😡", ":@" },
        };

        public Dictionary<string, string> AsciiToEmoji = new Dictionary<string, string>();

        public EmojiAsciiTranslator()
        {
            AsciiToEmoji = EmojisToAscii.ToDictionary(x => x.Value, x => x.Key);
        }

        public string TranslateEmojisToAscii(string str)
        {
            foreach(var kp in EmojisToAscii)
            {
                str = str.Replace(kp.Key, kp.Value);
            }
            return str;
        }

        public string TranslateAsciiToEmojis(string str)
        {
            foreach (var kp in AsciiToEmoji)
            {
                str = str.Replace(kp.Key, kp.Value);
            }
            return str;
        }

    }
}
