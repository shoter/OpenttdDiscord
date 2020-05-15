using Moq;
using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Database.Tests.Servers;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.AdminPort;
using OpenttdDiscord.Openttd.Tests.Network.AdminPort;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Backend.Tests.Admins
{
    public class AdminPortClientProviderTests
    {
        private readonly IAdminPortClientProvider provider;
        private readonly Mock<IServerService> serverService = new Mock<IServerService>();
        private readonly Mock<IAdminPortClientFactory> clientFactory = new Mock<IAdminPortClientFactory>();
        private readonly Mock<IAdminPortClientUser> defaultUserMock = new Mock<IAdminPortClientUser>();
        private IAdminPortClientUser DefaultUser => defaultUserMock.Object;

        public AdminPortClientProviderTests()
        {
            provider = new AdminPortClientProvider(serverService.Object, clientFactory.Object);
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => new AdminPortClientMockFixture().WithServerInfo(si).Build().Object);
        }

        [Fact]
        public async Task AfterRegister_YouShouldBeAbleToGetClient()
        {
            Server server = new ServerFixture().WithPassword("myPassword").Build();
            await provider.Register(DefaultUser, server);

            IAdminPortClient client = provider.GetClient(DefaultUser, server);
            Assert.Equal(client.ServerInfo.ServerIp, server.ServerIp);
            Assert.Equal(client.ServerInfo.ServerPort, server.ServerPort);
            Assert.Equal(client.ServerInfo.Password, server.ServerPassword);
        }

        [Fact]
        public void WhenUnregistered_YouShouldNotBeAbleToGetClient()
        {
            Server server = new ServerFixture().WithPassword("myPassword").Build();
            Assert.Throws<OttdException>(() => provider.GetClient(DefaultUser, server));
        }

        [Fact]
        public async Task AfterUnregistering_ItShouldDisconnectClient()
        {
            Mock<IAdminPortClient> client = null;
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);
            await provider.Register(DefaultUser, server);
            await provider.Unregister(DefaultUser, server);

            client.Verify(x => x.Disconnect(), Times.Once);
        }

        [Fact]
        public async Task Registering_ShouldNotCreateMoreThanOneAdminClientPerServer()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);
            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);
            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);
            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);

            clientFactory.Verify(x => x.Create(It.IsAny<ServerInfo>()), Times.Once);
        }

        [Fact]
        public async Task AfterUnregistering_IfMoreThanOneOwnerIsStillRegistered_ThenClientShouldNOTDisconnect()
        {
            Mock<IAdminPortClient> client = null;
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);

            await provider.Register(DefaultUser, server);
            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);
            await provider.Unregister(DefaultUser, server);

            client.Verify(x => x.Disconnect(), Times.Never);
        }

        [Fact]
        public async Task AfterPasswordChange_ServerShouldReceiveServerWithNewPassword_OnGetClient()
        {
            var sFixture = new ServerFixture().WithPassword("123");
            Server server = sFixture; 

            await provider.Register(DefaultUser, server);

            serverService.Raise(x => x.PasswordChanged += null, serverService.Object, sFixture.BasedOn(server).WithPassword("333").Build());

            IAdminPortClient client = provider.GetClient(DefaultUser, server);

            Assert.Equal("333", client.ServerInfo.Password);
            Assert.Equal(server.ServerIp, client.ServerInfo.ServerIp);
            Assert.Equal(server.ServerPort, client.ServerInfo.ServerPort);
        }

        [Fact]
        public async Task AfterUnregistering_GettingClientShouldGetException()
        {
            Server server = new ServerFixture().WithPassword("123").Build();

            await provider.Register(DefaultUser, server);
            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);
            await provider.Unregister(DefaultUser, server);

            Assert.Throws<OttdException>(() => provider.GetClient(DefaultUser, server));
        }

        [Fact]
        public async Task IfSomeoneUnregister_ItShouldNotAffectOtherOwners()
        {
            IAdminPortClientUser otherOwner = Mock.Of<IAdminPortClientUser>();
            Server server = new ServerFixture().WithPassword("123").Build();

            await provider.Register(DefaultUser, server);
            await provider.Register(otherOwner, server);
            await provider.Unregister(DefaultUser, server);
            var client = provider.GetClient(otherOwner, server);

            Assert.NotNull(client);
        }

        [Fact]
        public async Task UnregisteringThingsForUnregistered_ShouldDoNothing()
        {
            Mock<IAdminPortClient> client = null;
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);

            await provider.Register(Mock.Of<IAdminPortClientUser>(), server);
            await provider.Unregister(Mock.Of<IAdminPortClientUser>(), server);

            client.Verify(x => x.Disconnect(), Times.Never);
        }

        [Fact]
        public void BeAbleToTell_IfUserIsNotRegistered()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            Assert.False(provider.IsRegistered(Mock.Of<IAdminPortClientUser>(), server));
        }

        [Fact]
        public async Task BeAbleToTell_IfUserIsRegistered()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            await provider.Register(DefaultUser, server);
            Assert.True(provider.IsRegistered(DefaultUser, server)) ;
        }

        [Fact]
        public async Task BeAbleToTell_ThatUserIsUnregistered_AfterUnregisteringUser()
        {
            Server server = new ServerFixture().WithPassword("123").Build();
            await provider.Register(DefaultUser, server);
            await provider.Unregister(DefaultUser, server);
            Assert.False(provider.IsRegistered(DefaultUser, server));
        }

        [Fact]
        public async Task RegisteredUsers_ShouldBeInformedAboutServerEvents()
        {
            Mock<IAdminPortClient> client = null;
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);
            await provider.Register(DefaultUser, server);

            Mock<IAdminEvent> adminEventMock = new Mock<IAdminEvent>();
            adminEventMock.SetupGet(x => x.Server).Returns(new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword));
            client.Raise(x => x.EventReceived += null, server, adminEventMock.Object);
            defaultUserMock.Verify(x => x.ParseServerEvent(server, adminEventMock.Object), Times.Once);
        }

        [Fact]
        public async Task RegisteredUsers_ShouldNotBeInformedAboutServerEvents_AfterUnregistering()
        {
            Mock<IAdminPortClient> client = null;
            Server server = new ServerFixture().WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);
            await provider.Register(DefaultUser, server);
            await provider.Unregister(DefaultUser, server);

            Mock<IAdminEvent> adminEventMock = new Mock<IAdminEvent>();
            adminEventMock.SetupGet(x => x.Server).Returns(new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword));
            client.Raise(x => x.EventReceived += null, server, adminEventMock.Object);
            defaultUserMock.Verify(x => x.ParseServerEvent(server, adminEventMock.Object), Times.Never);
        }


        [Fact]
        public async Task RegisteredUsers_ShouldBeInformedAboutServerEvents_AfterServerPasswordChange()
        {
            Mock<IAdminPortClient> client = null;
            var sFixture = new ServerFixture();
            Server server = sFixture.WithPassword("123").Build();
            clientFactory.Setup(x => x.Create(It.IsAny<ServerInfo>())).Returns((ServerInfo si) => (client = new AdminPortClientMockFixture().WithServerInfo(si).Build()).Object);
            await provider.Register(DefaultUser, server);
            Server withPassChanged = sFixture.BasedOn(server).WithPassword("333").Build();
            serverService.Raise(x => x.PasswordChanged += null, serverService.Object, withPassChanged);

            Mock<IAdminEvent> adminEventMock = new Mock<IAdminEvent>();
            adminEventMock.SetupGet(x => x.Server).Returns(new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword));
            client.Raise(x => x.EventReceived += null, server, adminEventMock.Object);
            defaultUserMock.Verify(x => x.ParseServerEvent(withPassChanged, adminEventMock.Object), Times.Once);
            defaultUserMock.Verify(x => x.ParseServerEvent(server, adminEventMock.Object), Times.Never);
        }
    }
}
