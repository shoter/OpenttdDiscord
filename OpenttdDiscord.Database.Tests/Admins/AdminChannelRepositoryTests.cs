using OpenttdDiscord.Database.Admins;
using OpenttdDiscord.Testing;
using OpenttdDiscord.Testing.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OpenttdDiscord.Common;

namespace OpenttdDiscord.Database.Tests.Admins
{
    public class AdminChannelRepositoryTests : MysqlTest
    {
        public AdminChannelRepositoryTests() : base(new ContainerizedMysqlDatabase(), nameof(AdminChannelRepositoryTests))
        {
        }

        [Fact]
        public async Task AfterInsertingServer_WeShouldBeAbleToQueryAdminChannelsForGivenServer()
        {
            AdminChannelRepository repo = new AdminChannelRepository(GetMysql());

            await repo.Insert(DefaultTestData.DefaultServer, 11u);
            await repo.Insert(DefaultTestData.DefaultServer, 25u);
            await repo.Insert(DefaultTestData.DefaultServer, 33u);
            await repo.Insert(DefaultTestData.OtherServers.GetRandom(), 33u);

            ulong[] expectedChannels = new ulong[] { 11, 25, 33 };

            var channels = await repo.GetAdminChannels(DefaultTestData.DefaultServer);

            foreach(var e in expectedChannels)
            {
                Assert.NotNull(channels.Single(x => x.ChannelId == e && x.Server.Id == DefaultTestData.DefaultServer.Id));
            }
        }

        [Fact]
        public async Task AfterInsertingServer_WeShouldBeAbleToQueryAdminChannelsForGivenGuild()
        {
            AdminChannelRepository repo = new AdminChannelRepository(GetMysql());

            await repo.Insert(DefaultTestData.DefaultServer, 11u);
            await repo.Insert(DefaultTestData.DefaultServer, 25u);
            await repo.Insert(DefaultTestData.DefaultServer, 33u);
            await repo.Insert(DefaultTestData.OtherServers.GetRandom(), 33u);

            foreach(var s in DefaultTestData.SameGuildServers)
            {
                await repo.Insert(s, 11u);
            }

            var channels = await repo.GetAdminChannels(DefaultTestData.SameGuildServers[0].GuildId);

            foreach(var c in channels)
            {
                Assert.Equal(11u, c.ChannelId);
                Assert.Equal(DefaultTestData.SameGuildServers[0].GuildId, c.Server.GuildId);
                Assert.Single(channels, (AdminChannel ac) => ac.Server.Id == c.Server.Id);
            }
        }

        [Fact]
        public async Task ShouldBeAbleToRemoveServer()
        {
            AdminChannelRepository repo = new AdminChannelRepository(GetMysql());

            await repo.Insert(DefaultTestData.DefaultServer, 11u);
            await repo.Insert(DefaultTestData.DefaultServer, 25u);
            await repo.Insert(DefaultTestData.DefaultServer, 33u);
            await repo.Insert(DefaultTestData.OtherServers.GetRandom(), 33u);

            ulong[] expectedChannels = new ulong[] { 11, 25 };

            await repo.RemoveChannel(DefaultTestData.DefaultServer.Id, 33u);
            var channels = await repo.GetAdminChannels(DefaultTestData.DefaultServer);

            foreach (var e in expectedChannels)
            {
                Assert.NotNull(channels.Single(x => x.ChannelId == e && x.Server.Id == DefaultTestData.DefaultServer.Id));
            }
        }

    }
}
