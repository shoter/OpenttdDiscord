using System.Runtime.CompilerServices;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Rcon;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Servers;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Rcon
{
    public class RconChannelRepositoryShould : DatabaseBaseTest
    {
        public RconChannelRepositoryShould(PostgressDatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
        }

        [Fact]
        public async Task GetNoRconChannelsForGuild_IfNoRconChannelsWereInserted()
        {
            var repo = await CreateRpeository();

            var rconChannels = (await repo.GetRconChannelsForTheGuild(123u))
                .ThrowIfError().Right();
            Assert.Empty(rconChannels);
        }

        [Fact]
        public async Task GetNoRconChannelForServerIfNoRconChannelsWereInserted()
        {
            var repo = await CreateRpeository();

            var rconChannel = (await repo.GetRconChannel(Guid.NewGuid(), 123u))
                .ThrowIfError().Right();
            Assert.True(rconChannel.IsNone);
        }

        [Fact]
        public async Task InsertNewRconChannel()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var rconChannel = Fix.Create<RconChannel>() with
            {
                ServerId = server.Id
            };

            var retrieved = (await
                (from _1 in repo.Insert(rconChannel)
                 from rc in repo.GetRconChannel(server.Id, rconChannel.ChannelId)
                 select rc)).ThrowIfError().Right();

            Assert.True(retrieved.IsSome);
            var retrievedChatChannel = retrieved.Value();
            Assert.Equal(rconChannel, retrievedChatChannel);
        }

        [Fact]
        public async Task DeleteRconChannel()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var rconChannel = Fix.Create<RconChannel>() with
            {
                ServerId = server.Id
            };

            var retrieved = (await
                (from _1 in repo.Insert(rconChannel)
                 from _2 in repo.Delete(rconChannel.ServerId, rconChannel.ChannelId)
                 from rc in repo.GetRconChannel(server.Id, rconChannel.ChannelId)
                 select rc)).ThrowIfError().Right();

            Assert.True(retrieved.IsNone);
        }

        [Fact]
        public async Task Fail_WhenInsertingSameRconChannelTwice_ForSameServer()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var rconChannel = Fix.Create<RconChannel>() with
            {
                ServerId = server.Id
            };

            var result = await
                (from _1 in repo.Insert(rconChannel)
                 from _2 in repo.Insert(rconChannel)
                 from rc in repo.GetRconChannel(server.Id, rconChannel.ChannelId)
                 select rc);

            Assert.True(result.IsLeft);
        }

        private async Task<RconChannelRepository> CreateRpeository([CallerMemberName] string databaseName = "default")
        {
            var context = await CreateContext(databaseName);
            return new RconChannelRepository(context);
        }
    }
}
