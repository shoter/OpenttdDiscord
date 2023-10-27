using System.Runtime.CompilerServices;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Servers;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class OttdServerRepositoryShould : DatabaseBaseTest
    {
        public OttdServerRepositoryShould(PostgressDatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
        }

        [Fact]
        public async Task InsertServerToDatabase()
        {
            var repository = await CreateRpeository();
            var expectedServer = Fix.Create<OttdServer>();

            await repository.InsertServer(expectedServer);
            var retrievedServer = await repository.GetServer(expectedServer.Id);

            retrievedServer.Match(
                server => Assert.Equal(expectedServer, server),
                failure => throw new Exception(failure.Reason)
           );
        }

        [Fact]
        public async Task RemoveServerFromDatabase()
        {
            var repository = await CreateRpeository();
            var expectedServer = Fix.Create<OttdServer>();

            await repository.InsertServer(expectedServer);
            await repository.DeleteServer(expectedServer.Id);
            var retrievedServer = await repository.GetServer(expectedServer.Id);

            retrievedServer.Match(
                server => Assert.Equal(expectedServer, server),
                failure => throw new Exception(failure.Reason)
           );
        }

        [Fact]
        public async Task UpdateServerInDatabase()
        {
            var repository = await CreateRpeository();
            var insertedServer = Fix.Create<OttdServer>();

            await repository.InsertServer(insertedServer);
            await repository.DeleteServer(insertedServer.Id);

            var updatedServerr = Fix.Create<OttdServer>() with
            {
                Id = insertedServer.Id,
                GuildId = insertedServer.GuildId
            };

            await repository.UpdateServer(updatedServerr);

            var retrievedServer = await repository.GetServer(insertedServer.Id);

            retrievedServer.Match(
                server => Assert.Equal(updatedServerr, server),
                failure => throw new Exception(failure.Reason)
           );
        }

        private async Task<OttdServerRepository> CreateRpeository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new OttdServerRepository(context);
        }
    }
}
