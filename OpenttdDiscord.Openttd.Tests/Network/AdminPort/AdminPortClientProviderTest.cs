using Microsoft.Extensions.Logging;
using Moq;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Openttd.Tests.Network.AdminPort
{
    public class AdminPortClientProviderTest
    {
        private Mock<AdminPortClientFactory> clientFactoryMock = new Mock<AdminPortClientFactory>(Mock.Of<IAdminPacketService>(), Mock.Of<ILogger<IAdminPortClient>>());
        private IAdminPortClientProvider provider;

        public AdminPortClientProviderTest()
        {
            clientFactoryMock.CallBase = true;
            provider = new AdminPortClientProvider(clientFactoryMock.Object);
        }

        [Fact]
        public async Task GetClient_ShouldProvideCorrectClientOnFirstTry()
        {
            ServerInfo info = new ServerInfo("123", 123);
            IAdminPortClient client = await provider.GetClient(info);

            Assert.Equal(info, client.ServerInfo);
            // Client should not be trying to connect anywhere.
            Assert.Equal(AdminConnectionState.Idle, client.ConnectionState);
        }

        [Fact]
        public async Task GetClient_ShouldNotProvideNewClientOnSecondTry()
        {
            ServerInfo info = new ServerInfo("123", 123);
            IAdminPortClient client = await provider.GetClient(info);
            IAdminPortClient secondClient = await provider.GetClient(info);

            Assert.True(Object.ReferenceEquals(client, secondClient));
        }

        [Fact]
        public async Task GetClient_DifferentPasswordOnSecondTry_ShouldUpdatePassword()
        {
            ServerInfo info = new ServerInfo("123", 123);
            ServerInfo secondInfo = new ServerInfo("123", 123, "pass");

            IAdminPortClient client = await provider.GetClient(info);
            IAdminPortClient secondClient = await provider.GetClient(secondInfo);

            Assert.False(Object.ReferenceEquals(client, secondClient));
            Assert.Equal(string.Empty, client.ServerInfo.Password);
            Assert.Equal("pass", secondClient.ServerInfo.Password);
            Assert.Equal(string.Empty, info.Password);
        }

        [Theory]
        [InlineData(AdminConnectionState.Connected)]
        [InlineData(AdminConnectionState.Connecting)]
        [InlineData(AdminConnectionState.NotConnected)]

        public async Task GetClient_DifferentPasswordOnSecondTry_ShouldDisconnectFirstClient_IfItIsNotIdle(AdminConnectionState state)
        {
            ServerInfo info = new ServerInfo("123", 123);
            ServerInfo secondInfo = new ServerInfo("123", 123, "password");

            Mock<IAdminPortClient> firstClientMock = new Mock<IAdminPortClient>();
            firstClientMock.SetupGet(x => x.ServerInfo).Returns(info);
            Mock<IAdminPortClient> secondClientMock = new Mock<IAdminPortClient>();
            secondClientMock.SetupGet(x => x.ServerInfo).Returns(secondInfo);


            this.clientFactoryMock.Setup(x => x.Create(info)).Returns(firstClientMock.Object);
            IAdminPortClient client = await provider.GetClient(info);

            firstClientMock.SetupGet(x => x.ConnectionState).Returns(state);

            this.clientFactoryMock.Setup(x => x.Create(secondInfo)).Returns(secondClientMock.Object);
            IAdminPortClient secondClient = await provider.GetClient(secondInfo);

            firstClientMock.Verify(x => x.Disconnect(), Times.Once);
        }

        [Fact]
        public async Task GetClient_DifferentPasswordOnSecondTry_ShouldNOTDisconnectFirstClient_IfItWasNOTConnected()
        {
            ServerInfo info = new ServerInfo("123", 123);
            ServerInfo secondInfo = new ServerInfo("123", 123, "password");

            Mock<IAdminPortClient> firstClientMock = new Mock<IAdminPortClient>();
            firstClientMock.SetupGet(x => x.ServerInfo).Returns(info);
            Mock<IAdminPortClient> secondClientMock = new Mock<IAdminPortClient>();
            secondClientMock.SetupGet(x => x.ServerInfo).Returns(secondInfo);

            this.clientFactoryMock.Setup(x => x.Create(info)).Returns(firstClientMock.Object);
            IAdminPortClient client = await provider.GetClient(info);

            firstClientMock.SetupGet(x => x.ConnectionState).Returns(AdminConnectionState.Idle);


            this.clientFactoryMock.Setup(x => x.Create(secondInfo)).Returns(secondClientMock.Object);
            IAdminPortClient secondClient = await provider.GetClient(secondInfo);

            firstClientMock.Verify(x => x.Disconnect(), Times.Never);
        }
    }
}
