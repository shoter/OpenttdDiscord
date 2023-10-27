using Xunit;

namespace OpenttdDiscord.Database.Tests
{
    [CollectionDefinition(nameof(DatabaseCollection))]
    public class DatabaseCollection : ICollectionFixture<PostgressDatabaseFixture>
    {
    }
}
