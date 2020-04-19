using Moq;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class ServerServiceFixture
    {
        private IServerRepository serverRepository = Mock.Of<IServerRepository>();
        private ITimeProvider timeProvider = new TimeProvider();

        public ServerServiceFixture WithServerRepository(MySqlConfig config)
        {
            serverRepository = new ServerRepository(config);
            return this;
        }

        public ServerServiceFixture WithMockTime(out Mock<ITimeProvider> mock)
        {
            mock = new Mock<ITimeProvider>();
            this.timeProvider = mock.Object;
            return this;
        }

        public ServerService Build()
        {
            return new ServerService(serverRepository, timeProvider);
        }

        public static implicit operator ServerService(ServerServiceFixture fix) => fix.Build();
        
    }
}
