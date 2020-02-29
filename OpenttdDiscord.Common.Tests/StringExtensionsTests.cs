using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OpenttdDiscord.Common;

namespace OpenttdDiscord.Common.Tests
{
    public class StringExtensions
    {
        [InlineData("Test", "Test")]
        [InlineData("abc", "Abc")]
        [InlineData("abC", "Abc")]
        [InlineData("TEST", "Test")]
        [InlineData("TeSt", "Test")]
        [Theory]
        public void FirstUpper_TestCorrectness(string input, string expected)
        {
            Assert.Equal(expected, input.FirstUpper());
        }
    }
}
