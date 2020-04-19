using OpenttdDiscord.Database.Chatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Chatting
{
    public class ChatChannelServerServiceUnitTests
    {
        private readonly ChatChannelServerServiceFixture fix = new ChatChannelServerServiceFixture();

        [Fact]
        public void IsServerInRegisterProcess_ShouldReturnFalse_WhenNothingWasInserted()
        {
            ChatChannelServerService service = fix;

            Assert.False(service.IsServerInRegisterProcess(123u, "something", 123u));
        }

        [Fact]
        public void IsServerInRegisterProcess_ShouldReturnTrue_WhenProperServerWasInserted()
        {
            ChatChannelServerService service = fix;

            service.InformAboutNewChannelInRegisterProcess(new InRegisterChatChannelServer
            {
                ChannelId = 30u,
                UserId = 40u,
                ServerName = "test"
            });

            Assert.True(service.IsServerInRegisterProcess(40u, "test", 30u));
        }

        [Fact]
        public void IsServerInRegisterProcess_ShouldReturnFalse_WhenEntryWillBeExpired()
        {
            ChatChannelServerService service = fix.WithMockTimeProvider(out var timeMock);
            DateTime now = DateTime.Now;

            timeMock.SetupGet(x => x.Now).Returns(now);
            service.InformAboutNewChannelInRegisterProcess(new InRegisterChatChannelServer
            {
                ChannelId = 30u,
                UserId = 40u,
                ServerName = "test"
            });
            timeMock.SetupGet(x => x.Now).Returns(now
                .Add(InRegisterChatChannelServer.DefaultExpiryTime)
                .AddMinutes(1));

            Assert.False(service.IsServerInRegisterProcess(40u, "test", 30u));
        }
    }
}
