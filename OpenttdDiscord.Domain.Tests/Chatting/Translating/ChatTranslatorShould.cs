using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Chatting.Translating;

namespace OpenttdDiscord.Domain.Tests.Chatting.Translating
{
    public class ChatTranslatorShould
    {
        private readonly IEmojiTranslator emojiTranslator = new EmojiTranslator();
        private readonly IChatTranslator sut;

        public ChatTranslatorShould()
        {
            sut = new ChatTranslator(emojiTranslator);
        }

        [Theory]
        [InlineData("😠", ">:(")]
        [InlineData("😛", ":P")]
        [InlineData("🙂", ":)")]
        [InlineData("😄", ":D")]
        [InlineData("😦", ":(")]
        [InlineData("😭", ";(")]
        [InlineData("😮", ":O")]
        [InlineData("😉", ";)")]
        [InlineData("😐", ":|")]
        [InlineData("❤️", "<3")]
        [InlineData("😡", ":@")]
        public void ProperlyTranslateEmojiToAscii(string emoji, string ascii)
        {
            Assert.Equal(
                ascii,
                sut.FromDiscordToOttd(emoji).Right());
        }

        [Theory]
        [InlineData("😠", ">:(")]
        [InlineData("😛", ":P")]
        [InlineData("🙂", ":)")]
        [InlineData("😄", ":D")]
        [InlineData("😦", ":(")]
        [InlineData("😭", ";(")]
        [InlineData("😮", ":O")]
        [InlineData("😉", ";)")]
        [InlineData("😐", ":|")]
        [InlineData("❤️", "<3")]
        [InlineData("😡", ":@")]
        public void ProperlyTranslateAsciiToEmoji(string emoji, string ascii)
        {
            Assert.Equal(
                emoji,
                sut.FromOttdToDiscord(ascii).Right());
        }

        [Theory]
        [InlineData("Super text\nYeah...", "Super text.Yeah...")]
        public void ShouldReplaceNewLines_WithDots(string input, string expectedOutput)
        {
            Assert.Equal(
                expectedOutput,
                sut.FromDiscordToOttd(expectedOutput).Right());
        }
    }
}
