using AutoFixture;
using DotNet.Testcontainers.Builders;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Domain.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Servers
{
    public class OttdServerRepositoryShould : DatabaseBaseTest
    {
        public OttdServerRepositoryShould(PostgressDatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task InsertServerToDatabase()
        {
            var repository = await CreateRpeository();
            var expectedServer = fix.Create<OttdServer>();

            await repository.InsertServer(expectedServer);
            var retrievedServer = (await repository.GetServer(expectedServer.Id));

            retrievedServer.Match(
                server => Assert.Equal(expectedServer, server),
                failure => throw new Exception(failure.Reason)
           );
        }

        [Fact]
        public async Task RemoveServerFromDatabase()
        {
            var repository = await CreateRpeository();
            var expectedServer = fix.Create<OttdServer>();

            await repository.InsertServer(expectedServer);
            await repository.DeleteServer(expectedServer.Id);
            var retrievedServer = (await repository.GetServer(expectedServer.Id));

            retrievedServer.Match(
                server => Assert.Equal(expectedServer, server),
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
