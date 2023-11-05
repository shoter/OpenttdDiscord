using System.Runtime.CompilerServices;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.AutoReplies;
using OpenttdDiscord.Domain.AutoReplies;
using Xunit;

namespace OpenttdDiscord.Database.Tests.AutoReplies
{
    public class AutoReplyRepositoryShould : DatabaseBaseTest
    {
        public AutoReplyRepositoryShould(PostgressDatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
        }

        [Fact]
        public async Task UpdateWelcomeMessage()
        {
            var server = await CreateServer();
            var repo = await CreateRepository();
            var message = Fix.Create<WelcomeMessage>() with
            {
                ServerId = server.Id,
            };
            var expectedMessage = message with { Content = "2137" };

            var updatedMessage =
                await (from _1 in repo.UpsertWelcomeMessage(
                        server.GuildId,
                        message)
                    from _2 in repo.UpsertWelcomeMessage(
                        server.GuildId,
                        expectedMessage)
                    from msg in repo.GetWelcomeMessage(
                        server.GuildId,
                        server.Id)
                    select msg);

            Assert.Equal(
                expectedMessage,
                updatedMessage.Right());
        }

        [Fact]
        public async Task InsertWelcomeMessage_AndRetrieveIt()
        {
            var server = await CreateServer();
            var repo = await CreateRepository();
            var message = Fix.Create<WelcomeMessage>() with
            {
                ServerId = server.Id,
            };

            var updatedMessage =
                await (from _1 in repo.UpsertWelcomeMessage(
                        server.GuildId,
                        message)
                    from msg in repo.GetWelcomeMessage(
                        server.GuildId,
                        server.Id)
                    select msg);

            Assert.Equal(
                message,
                updatedMessage.Right());
        }

        /// <remarks>
        /// There was a bug where GuildId was a PK for welcome message. This prevented people from adding more than 1 welcome message per guid.
        /// </remarks>
        [Fact]
        public async Task InsertMoreThanOneWelcomeMessagePerGuid()
        {
            var guildId = Fix.Create<ulong>();
            var server = await CreateServer(customize: s => s with { GuildId = guildId });
            var server2 = await CreateServer(customize: s => s with { GuildId = guildId });
            var repo = await CreateRepository();
            var message = Fix.Create<WelcomeMessage>() with
            {
                ServerId = server.Id,
            };
            var message2 = Fix.Create<WelcomeMessage>() with { ServerId = server2.Id };

            var updatedMessage =
                await (from _1 in repo.UpsertWelcomeMessage(
                        server.GuildId,
                        message)
                    from _2 in repo.UpsertWelcomeMessage(
                        server2.GuildId,
                        message2)
                    from msg in repo.GetWelcomeMessage(
                        server.GuildId,
                        server.Id)
                    select msg);
            var updatedMessage2 =
                await (from msg in repo.GetWelcomeMessage(
                    server2.GuildId,
                    server2.Id)
                select msg);

            Assert.Equal(
                message,
                updatedMessage.Right().Case);
            Assert.Equal(
                message2,
                updatedMessage2.Right().Case);
        }

        private async Task<AutoReplyRepository> CreateRepository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new AutoReplyRepository(context);
        }
    }
}