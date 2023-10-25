using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoFixture;
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
    }
}
