using System.Runtime.CompilerServices;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Servers;
using Xunit;

namespace OpenttdDiscord.Database.Tests
{
    [Collection(nameof(DatabaseCollection))]
    public class DatabaseBaseTest
    {
        private readonly PostgressDatabaseFixture databaseFixture;

        internal readonly Fixture Fix = new();

        public DatabaseBaseTest(PostgressDatabaseFixture databaseFixture)
        {
            this.databaseFixture = databaseFixture;
        }

        internal Task<OttdContext> CreateContext([CallerMemberName] string? databaseName = null)
        {
            if (databaseName == null)
            {
                throw new ArgumentNullException(databaseName);
            }

            return databaseFixture.CreateContext(databaseName);
        }

        protected async Task<OttdServer> CreateServer([CallerMemberName] string? databaseName = null, Func<OttdServer, OttdServer>? customize = null)
        {
            var context = await CreateContext(databaseName);
            var repository = new OttdServerRepository(context);
            var server = Fix.Create<OttdServer>();
            server = customize?.Invoke(server);
            (await repository.InsertServer(server)).ThrowIfError();
            return server;
        }
    }
}
