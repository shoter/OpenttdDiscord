using System.Runtime.CompilerServices;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Reporting;
using OpenttdDiscord.Domain.Servers;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Reporting
{
    public class ReportChannelRepositoryShould : DatabaseBaseTest
    {
        public ReportChannelRepositoryShould(PostgressDatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
        }

        [Fact]
        public async Task GetNoChatChannels_IfNoChatChannelsWereInserted()
        {
            var repo = await CreateRpeository();

            var chatChannels = await repo.GetReportChannelsForServer(Guid.NewGuid());
            chatChannels.ThrowIfError();
            Assert.Empty(chatChannels.Right());
        }

        [Fact]
        public async Task InsertNewChatChannel()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var chatChannel = Fix.Create<ReportChannel>() with
            {
                ServerId = server.Id
            };

            var chatChannels = await
                (from _1 in repo.Insert(chatChannel)
                 from rc in repo.GetReportChannelsForServer(server.Id)
                 select rc);

            chatChannels.ThrowIfError();
            Assert.Single(chatChannels.Right());
            var retrievedChatChannel = chatChannels.Right().First();
            Assert.Equal(chatChannel, retrievedChatChannel);
        }

        [Fact]
        public async Task DeleteChatChannel()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var chatChannel = Fix.Create<ReportChannel>() with
            {
                ServerId = server.Id
            };

            var chatChannels = await
                (from _1 in repo.Insert(chatChannel)
                 from _2 in repo.Delete(server.Id, chatChannel.ChannelId)
                 from rc in repo.GetReportChannelsForServer(server.Id)
                 select rc);

            chatChannels.ThrowIfError();
            Assert.Empty(chatChannels.Right());
        }

        [Fact]
        public async Task Fail_WhenInsertingSameChatChannelTwice_ForSameServer()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var chatChannel = Fix.Create<ReportChannel>() with
            {
                ServerId = server.Id
            };

            var chatChannels = await
                (from _1 in repo.Insert(chatChannel)
                 from _2 in repo.Insert(chatChannel)
                 from rc in repo.GetReportChannelsForServer(server.Id)
                 select rc);

            Assert.True(chatChannels.IsLeft);
        }

        private async Task<ReportChannelRepository> CreateRpeository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new ReportChannelRepository(context);
        }
    }
}
