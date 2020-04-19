using Moq;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Tests.Chatting
{
    public class ChatChannelServerServiceFixture
    {
        private IChatChannelServerRepository repository = new Mock<IChatChannelServerRepository>().Object;
        private IServerService serverService = Mock.Of<IServerService>();
        private ITimeProvider timeProvider = Mock.Of<ITimeProvider>();

        public ChatChannelServerServiceFixture()
        {
            WithMockTimeProvider(out _);
        }

        public ChatChannelServerServiceFixture WithMockRepository(out Mock<IChatChannelServerRepository> mock )
        {
            mock = new Mock<IChatChannelServerRepository>();
            repository = mock.Object;
            return this;
        }

        public ChatChannelServerServiceFixture WithMockServerService(out Mock<IServerService> mock)
        {
            mock = new Mock<IServerService>();
            serverService = mock.Object;
            return this;
        }

        public ChatChannelServerServiceFixture WithMockTimeProvider(out Mock<ITimeProvider> mock)
        {
            mock = new Mock<ITimeProvider>();
            mock.SetupGet(x => x.Now).Returns(DateTime.Now);
            timeProvider = mock.Object;
            return this;
        }

        public static implicit operator ChatChannelServerService(ChatChannelServerServiceFixture fix) =>
            new ChatChannelServerService(fix.repository, fix.serverService, fix.timeProvider);


    }
}
