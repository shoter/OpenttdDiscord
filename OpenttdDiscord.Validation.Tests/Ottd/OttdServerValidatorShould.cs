using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Validation.Ottd;
using System;
using Xunit;

namespace OpenttdDiscord.Validation.Tests.Ottd
{
    public class OttdServerValidatorShould
    {
        private OttdServer correctServer = new OttdServer(
            Guid.NewGuid(),
            123,
            "127.0.0.1",
            "SuperServer",
            1234,
            "psst-secretPassword");

        private OttdServerValidator validator = new();

        [Theory]
        [InlineData(0)]
        [InlineData(65536)]
        [InlineData(2137)]
        [InlineData(420)]
        [InlineData(69)]
        public void AllowCorrectPorts_ForAdminPort(int port)
        {
            var server = correctServer with { AdminPort = port };
            var result = validator.Validate(server);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(65537)]
        public void DisallowIncorrectPort_ForAdminPort(int port)
        {
            var server = correctServer with { AdminPort = port };
            var result = validator.Validate(server);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("1.2.3.4")]
        [InlineData("21.37.69.42")]
        // v6
        [InlineData("684D:1111:222:3333:4444:5555:6:77")]
        [InlineData("::")]
        [InlineData("2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF")]
        [InlineData("2001:db8::")]
        [InlineData("2001:0db8:0001:0000:0000:0ab9:C0A8:0102")]
        [InlineData("2001:db8:1::ab9:C0A8:102")]
        public void AllowCorrectIpAddress(string ip)
        {
            var server = correctServer with { Ip = ip };
            var result = validator.Validate(server);
            Assert.True(result.IsValid);
        }

        [Theory]
        // First add space here and there. Validator is not doing any trimming. Exact string need to pass ip address validation.
        [InlineData("1.2.3.4 ")]
        [InlineData("21.37.69.42 ")]
        // v6
        [InlineData(" 684D:1111:222:3333:4444:5555:6:77")]
        [InlineData(":: ")]
        [InlineData(" 2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF")]
        [InlineData("2001:db8:: ")]
        [InlineData(" 2001:0db8:0001:0000:0000:0ab9:C0A8:0102")]
        [InlineData("2001:db8:1::ab9:C0A8:102 ")]
        // new data
        [InlineData("That's not an ip address. :D")]
        [InlineData("256.256.256.256")]
        [InlineData("....")]
        [InlineData(":::")]
        public void DisallowCorrectIpAddress(string ip)
        {
            var server = correctServer with { Ip = ip };
            var result = validator.Validate(server);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void DisallowEmptyPassword_WhenPortIsPresent()
        {
            var server = correctServer with { AdminPortPassword = "" };
            var result = validator.Validate(server);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("12348h23rg8912348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9b12348h23rg89rebe9rvbe9brebe9rvbe9b")]
        public void DisallowWrongNames(string name)
        {
            var server = correctServer with { Name = name };
            var result = validator.Validate(server);
            Assert.False(result.IsValid);
        }
    }
}
