using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
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
                .WithName("moja_dupa")
                .WithImage("ottd_discord_test_database")
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithPortBinding(5432, true)
                .Build();

            await container.StartAsync().ConfigureAwait(false);

            var port = container.GetMappedPublicPort(5432);

            var connectionString = "User ID=openttd;" +
                "Password=secret-pw;" +
                $"Host=localhost;Port={port};" +
                "Database=test;Pooling=true;Min Pool Size=0;" +
                "Max Pool Size=100;Connection Lifetime=0;";

            var optionsBuilder = new DbContextOptionsBuilder<OttdContext>();
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable(connectionString), x =>
            {
                x.MigrationsHistoryTable("__MigrationHistory");
            });

            var context = new OttdContext(optionsBuilder.Options);

            await context.Database.EnsureCreatedAsync();

            int a = 5;
        }
    }
}
