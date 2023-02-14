using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests
{
    [Collection(nameof(DatabaseCollection))]
    public class DatabaseBaseTest
    {

        private readonly PostgressDatabaseFixture databaseFixture;

        internal readonly Fixture fix = new();

        public DatabaseBaseTest(PostgressDatabaseFixture databaseFixture)
        {
            this.databaseFixture = databaseFixture;
        }

        internal Task<OttdContext> CreateContext([CallerMemberName]string? databaseName = null)
        {
            if(databaseName == null)
            {
                throw new ArgumentNullException(databaseName);
            }

            return databaseFixture.CreateContext(databaseName);
        }
    }
}
