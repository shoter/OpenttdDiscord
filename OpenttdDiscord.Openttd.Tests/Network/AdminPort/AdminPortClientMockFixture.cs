using Moq;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd.Tests.Network.AdminPort
{
    public class AdminPortClientMockFixture
    {
        private ServerInfo serverInfo;

        public Mock<IAdminPortClient> Build()
        {
            Mock<IAdminPortClient> mock = new Mock<IAdminPortClient>();
            mock.SetupGet(x => x.ServerInfo).Returns(serverInfo);
            mock.SetupGet(x => x.ConnectionState).Returns(AdminConnectionState.Idle);
            mock.Setup(x => x.Join()).Returns(Task.CompletedTask).Callback(() => mock.SetupGet(x => x.ConnectionState).Returns(AdminConnectionState.Connected));
            mock.Setup(x => x.Disconnect()).Returns(Task.CompletedTask).Callback(() => mock.SetupGet(x => x.ConnectionState).Returns(AdminConnectionState.Idle));
            return mock;
        }

        public AdminPortClientMockFixture WithServerInfo(ServerInfo si)
        {
            this.serverInfo = si;
            return this;
        }

        
    }
}
