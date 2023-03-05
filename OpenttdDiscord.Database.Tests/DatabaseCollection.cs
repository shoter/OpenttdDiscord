using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests
{
    [CollectionDefinition(nameof(DatabaseCollection))]
    public class DatabaseCollection : ICollectionFixture<PostgressDatabaseFixture>
    {
    }
}
