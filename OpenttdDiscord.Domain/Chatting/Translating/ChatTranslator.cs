using System.Text;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Chatting.Translating
{
    public class ChatTranslator : IChatTranslator
    {
        private readonly IEmojiTranslator emojiTranslator;

        public ChatTranslator(IEmojiTranslator emojiTranslator)
        {
            this.emojiTranslator = emojiTranslator;
        }

        public Either<IError, string> FromDiscordToOttd(string input)
        {
            StringBuilder sb = new(input);

            return
                from _1 in emojiTranslator.FromDiscordToOttd(sb)
                select sb.ToString();
        }

        public Either<IError, string> FromOttdToDiscord(string input)
        {
            StringBuilder sb = new(input);

            return
                from _1 in emojiTranslator.FromOttdToDiscord(sb)
                select sb.ToString();
        }
    }
}
