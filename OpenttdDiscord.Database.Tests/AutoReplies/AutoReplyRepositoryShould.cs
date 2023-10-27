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
                await (from _1 in repo.InsertWelcomeMessage(
                        server.GuildId,
                        message)
                    from _2 in repo.UpdateWelcomeMessage(
                        server.GuildId,
                        message)
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
                await (from _1 in repo.InsertWelcomeMessage(
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

        private async Task<AutoReplyRepository> CreateRepository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new AutoReplyRepository(context);
        }
    }
}