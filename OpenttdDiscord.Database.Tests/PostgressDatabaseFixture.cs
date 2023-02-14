using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Tests
{
    public class PostgressDatabaseFixture : IDisposable
    {
        private IContainer container;

        public PostgressDatabaseFixture()
        {
            container = new ContainerBuilder()
            .WithImage("ottd_discord_test_database")
            .WithPortBinding(5432, true)
            .WithCommand("-c", "fsync=off")
            .WithCommand("-c", "full_page_writes=off")
            .WithCommand("-c", "synchronous_commit=off")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .Build();

            container.StartAsync().Wait();
        }

        internal async Task<OttdContext> CreateContext(string databaseName)
        {
            var port = container.GetMappedPublicPort(5432);

            var connectionString =
                 $"Host=localhost;Port={port};" +
                 "User ID=openttd;" +
                 $"Database={databaseName};" +
                 "Password=secret-pw;";

            var optionsBuilder = new DbContextOptionsBuilder<OttdContext>();
            optionsBuilder.UseNpgsql(connectionString, x =>
            {
                x.MigrationsHistoryTable("__MigrationHistory");
            });

            var context = new OttdContext(optionsBuilder.Options);

            await context.Database.EnsureCreatedAsync();

            return context;
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PostgressDatabaseContext()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
