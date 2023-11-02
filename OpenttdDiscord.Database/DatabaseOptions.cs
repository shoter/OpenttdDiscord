using System.Diagnostics.CodeAnalysis;

namespace OpenttdDiscord.Database
{
    [ExcludeFromCodeCoverage]

    public class DatabaseOptions
    {
        public string ConnectionString { get; set; } = default!;
    }
}
