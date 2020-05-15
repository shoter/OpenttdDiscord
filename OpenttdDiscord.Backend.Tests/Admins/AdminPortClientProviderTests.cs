using Moq;
using NLog.Targets.Wrappers;
using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Database.Tests.Servers;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.AdminPort;
using OpenttdDiscord.Openttd.Tests.Network.AdminPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Backend.Tests.Admins
{
    public class AdminPortClientProviderTests
    {
        private IAdminPortClientProvider provider;
        private Mock<IServerService> serverService = new Mock<IServerService>();
        private Mock<IAdminPortClientFactory> clientFactory = new Mock<IAdminPortClientFactory>();

        public AdminPortClientProviderTests()
        {
            provider = new AdminPortClientProvider(serverService.Object, clientFactory.Object);
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => new AdminPortClientMockFixture().WithServerInfo(si).Build().Object);
        }

        [Fact]
        public async Task AfterRegister_YouShouldBeAbleToGetClient()
        {
            object owner = new object();
            Server server = new ServerFixture().WithPassword("myPassword").Build();
            await provider.Register(owner, server);

            IAdminPortClient client = provider.GetClient(owner, server);
            Assert.Equal(client.ServerInfo.ServerIp, server.ServerIp);
            Assert.Equal(client.ServerInfo.ServerPort, server.ServerPort);
            Assert.Equal(client.ServerInfo.Password, server.ServerPassword);
        }

        [Fact]
        public void WhenUnregistered_YouShouldNotBeAbleToGetClient()
        {
            object owner = new object();
            Server server = new ServerFixture().WithPassword("myPassword").Build();
            Assert.Throws<OttdException>(() => provider.GetClient(owner, server));
        }

        [Fact]
        public async Task AfterUnregistering_ItShouldDisconnectClient()
        {
            Mock<IAdminPortClient> client = null;
            object owner = new object();
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);
            await provider.Register(owner, server);
            await provider.Unregister(owner, server);

            client.Verify(x => x.Disconnect(), Times.Once);
        }

        [Fact]
        public async Task Registering_ShouldNotCreateMoreThanOneAdminClientPerServer()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            await provider.Register(new object(), server);
            await provider.Register(new object(), server);
            await provider.Register(new object(), server);
            await provider.Register(new object(), server);

            clientFactory.Verify(x => x.Create(It.IsAny<ServerInfo>()), Times.Once);
        }

        [Fact]
        public async Task AfterUnregistering_IfMoreThanOneOwnerIsStillRegistered_ThenClientShouldNOTDisconnect()
        {
            Mock<IAdminPortClient> client = null;
            object owner = new object();
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);

            await provider.Register(owner, server);
            await provider.Register(new object(), server);
            await provider.Unregister(owner, server);

            client.Verify(x => x.Disconnect(), Times.Never);
        }

        [Fact]
        public async Task AfterPasswordChange_ServerShouldReceiveServerWithNewPassword_OnGetClient()
        {
            object owner = new object();
            var sFixture = new ServerFixture().WithPassword("123");
            Server server = sFixture; 

            await provider.Register(owner, server);

            serverService.Raise(x => x.PasswordChanged += null, serverService.Object, sFixture.WithPassword("333").Build());

            IAdminPortClient client = provider.GetClient(owner, server);

            Assert.Equal("333", client.ServerInfo.Password);
            Assert.Equal(server.ServerIp, client.ServerInfo.ServerIp);
            Assert.Equal(server.ServerPort, client.ServerInfo.ServerPort);
        }

        [Fact]
        public async Task AfterUnregistering_GettingClientShouldGetException()
        {
            object owner = new object();
            Server server = new ServerFixture().WithPassword("123").Build();

            await provider.Register(owner, server);
            await provider.Register(new object(), server);
            await provider.Unregister(owner, server);

            Assert.Throws<OttdException>(() => provider.GetClient(owner, server));
        }

        [Fact]
        public async Task IfSomeoneUnregister_ItShouldNotAffectOtherOwners()
        {
            object owner = new object();
            object otherOwner = new object();
            Server server = new ServerFixture().WithPassword("123").Build();

            await provider.Register(owner, server);
            await provider.Register(otherOwner, server);
            await provider.Unregister(owner, server);
            var client = provider.GetClient(otherOwner, server);

            Assert.NotNull(client);
        }

        [Fact]
        public async Task UnregisteringThingsForUnregistered_ShouldDoNothing()
        {
            Mock<IAdminPortClient> client = null;
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);

            await provider.Register(new object(), server);
            await provider.Unregister(new object(), server);

            client.Verify(x => x.Disconnect(), Times.Never);
        }

        [Fact]
        public void BeAbleToTell_IfUserIsNotRegistered()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            Assert.False(provider.IsRegistered(new object(), server));
        }

        [Fact]
        public async Task BeAbleToTell_IfUserIsRegistered()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            object user = new object();
            await provider.Register(user, server);
            Assert.True(provider.IsRegistered(new object(), server));
        }

        [Fact]
        public async Task BeAbleToTell_ThatUserIsUnregistered_AfterUnregisteringUser()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            object user = new object();
            await provider.Register(user, server);
            await provider.Unregister(user, server);
            Assert.False(provider.IsRegistered(new object(), server));
        }


    }
}
