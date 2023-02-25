using AutoFixture;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Chatting;
using OpenttdDiscord.Domain.Servers;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Chatting
{
    public class ChatChannelRepositoryShould : DatabaseBaseTest
    {
        public ChatChannelRepositoryShould(PostgressDatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task GetNoChatChannels_IfNoChatChannelsWereInserted()
        {
            var repo = await CreateRpeository();

            var chatChannels = await repo.GetChatChannelsForServer(Guid.NewGuid());
            chatChannels.ThrowIfError();
            Assert.Empty(chatChannels.Right());
        }

        [Fact]
        public async Task InsertNewChatChannel()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var chatChannel = fix.Create<ChatChannel>() with
            {
                ServerId = server.Id
            };

            var chatChannels = await
                (from _1 in repo.Insert(chatChannel)
                from cc in repo.GetChatChannelsForServer(server.Id)
                select cc);

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
            var chatChannel = fix.Create<ChatChannel>() with
            {
                ServerId = server.Id
            };

            var chatChannels = await
                (from _1 in repo.Insert(chatChannel)
                 from _2 in repo.Delete(server.Id, chatChannel.ChannelId)
                 from cc in repo.GetChatChannelsForServer(server.Id)
                 select cc);

            chatChannels.ThrowIfError();
            Assert.Empty(chatChannels.Right());
        }

        [Fact]
        public async Task Fail_WhenInsertingSameChatChannelTwice_ForSameServer()
        {
            var repo = await CreateRpeository();
            var server = await CreateServer();
            var chatChannel = fix.Create<ChatChannel>() with
            {
                ServerId = server.Id
            };

            var chatChannels = await
                (from _1 in repo.Insert(chatChannel)
                 from _2 in repo.Insert(chatChannel)
                 from cc in repo.GetChatChannelsForServer(server.Id)
                 select cc);

            Assert.True(chatChannels.IsLeft);
        }

        private async Task<ChatChannelRepository> CreateRpeository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new ChatChannelRepository(context);
        }

        private async Task<OttdServer> CreateServer([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            var repository = new OttdServerRepository(context);
            var server = fix.Create<OttdServer>();
            (await repository.InsertServer(server)).ThrowIfError();
            return server;
        }
    }
}
