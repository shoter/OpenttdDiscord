using Moq;
using Moq.Internals;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class ServerServiceUnitTests
    {
        public ServerServiceFixture fix = new ServerServiceFixture();

        [Fact]
        public void IsPasswordRequestInProgress_ShouldReturnFalse_WhenNothingWasDone()
        {
            ServerService service = fix;

            Assert.False(service.IsPasswordRequestInProgress(123u));
        }

        [Fact]
        public void IsPasswordRequestInProgress_ShouldReturnTrue_WhenProperServerWasInserted()
        {
            ServerService service = fix;

            service.InformAboutNewPasswordRequest(new NewServerPassword
            {
                ServerName = "test",
                UserId = 12u,
                GuildId = 11u,
            });

            Assert.True(service.IsPasswordRequestInProgress(12u));
        }

        [Fact]
        public void IsPasswordRequestInProgress_ShouldReturnFalse_WhenDifferentUserIsResetingPassword()
        {
            ServerService service = fix;

            service.InformAboutNewPasswordRequest(new NewServerPassword
            {
                ServerName = "test",
                UserId = 12u,
                GuildId = 11u,
            });

            Assert.False(service.IsPasswordRequestInProgress(14u));
        }

        [Fact]
        public void InformAboutNewPasswordRequest_TwoUsers_ShouldBeAbleToUseItAtTheSameTime()
        {
            ServerService service = fix;

            service.InformAboutNewPasswordRequest(new NewServerPassword
            {
                ServerName = "test",
                UserId = 12u,
                GuildId = 11u,
            });

            service.InformAboutNewPasswordRequest(new NewServerPassword
            {
                ServerName = "test",
                UserId = 13u,
                GuildId = 11u
            });

            Assert.True(service.IsPasswordRequestInProgress(13u));
            Assert.True(service.IsPasswordRequestInProgress(12u));
        }

        [Fact]
        public void GetNewPasswordProcess_ShouldReturnInformationAboutExistingProcess()
        {
            ServerService service = fix;

            service.InformAboutNewPasswordRequest(new NewServerPassword
            {
                ServerName = "test",
                UserId = 13u,
                GuildId = 11u,
            });

            var nsp = service.GetNewPasswordProcess(13u);

            Assert.Equal("test", nsp.ServerName);
            Assert.Equal(13u, nsp.UserId);
        }

        [Fact]
        public void IsPasswordRequestInProgress_ShouldReturnFalse_WhenEntryWillBeExpired()
        {
            ServerService service = fix.WithMockTime(out var timeMock);
            DateTime now = DateTime.Now;

            timeMock.SetupGet(x => x.Now).Returns(now);

            service.InformAboutNewPasswordRequest(new NewServerPassword
            {
                ServerName = "test",
                UserId = 12u,
                GuildId = 11u,
            });

            timeMock.SetupGet(x => x.Now).Returns(now
            .Add(NewServerPassword.DefaultExpiryTime)
            .AddMinutes(1));


            Assert.False(service.IsPasswordRequestInProgress(12u));
        }

        [Fact]
        public async Task PasswordChanged_ShouldBeInvoked_WhenPasswordForSpecificServerWasChanged()
        {
            ServerService ss = fix.WithMockServerRepository(out var repo);
            Server s = new ServerFixture();

            int order = 0;

            repo.Setup(x => x.UpdatePassword(s.Id, It.IsAny<string>())).Callback(() => Assert.Equal(0, order++));
            repo.Setup(x => x.GetServer(s.Id)).Callback(() => Assert.Equal(1, order)).ReturnsAsync(s);

            Server eventServer = null;
            ss.PasswordChanged += (_, es) => eventServer = es;
            await ss.ChangePassword(s.Id, "newPassword");

            Assert.Equal(s, eventServer);
        }
    }
}
