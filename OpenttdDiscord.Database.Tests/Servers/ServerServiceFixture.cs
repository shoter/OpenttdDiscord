using Moq;
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

        public ServerServiceFixture WithServerRepository(MySqlConfig config)
        {
            serverRepository = new ServerRepository(config);
            return this;
        }

        public ServerService Build()
        {
            return new ServerService(serverRepository);
        }

        public static implicit operator ServerService(ServerServiceFixture fix) => fix.Build();
        
    }
}
