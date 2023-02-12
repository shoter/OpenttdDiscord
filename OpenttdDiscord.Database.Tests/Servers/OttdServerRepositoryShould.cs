using DotNet.Testcontainers.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class OttdServerRepositoryShould
    {
        [Fact]
        public async Task test()
        {
            var container = new ContainerBuilder()
                .WithImage("ottd_discord_test_database")
                .WithAutoRemove(true)
                .WithPortBinding(5432, assignRandomHostPort: true)
                .Build();

            var _ = container.StartAsync();
        }
    }
}
