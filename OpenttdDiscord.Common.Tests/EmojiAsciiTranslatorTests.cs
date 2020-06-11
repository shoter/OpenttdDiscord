using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Common.Tests
{
    public class EmojiAsciiTranslatorTests
    {
        EmojiAsciiTranslator translator = new EmojiAsciiTranslator();

        [Fact]
        public void ShouldBeAbleToTranslateEmojisIntoText()
        {
            string emojis = "test 😛 to 🙂 jest 😄";

            Assert.Equal("test :P to :) jest :D", translator.TranslateEmojisToAscii(emojis));
        }

        [Fact]
        public void ShouldBeAbleToTranslateTextIntoEmojis()
        {
            string emojis = "test :P to :) jest :D >:(";

            Assert.Equal("test 😛 to 🙂 jest 😄 😠", translator.TranslateAsciiToEmojis(emojis));
        }



    }
}
