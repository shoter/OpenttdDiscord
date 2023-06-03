using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoFixture;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Roles;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Servers;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Roles
{
    public class RolesRepositoryShould : DatabaseBaseTest
    {
        public RolesRepositoryShould(PostgressDatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
        }

        [Fact]
        public async Task InsertServerToDatabase()
        {
            var repository = await CreateRpeository();
            var expectedRole = Fix.Create<GuildRole>();

            var roles =
                (from _1 in repository.InsertRole(expectedRole)
                    from roleList in repository.GetRoles(expectedRole.GuildId)
                    select roleList).Right();

            Assert.Single(roles);
            Assert.Equal(
                expectedRole,
                roles.First());
        }

        [Fact]
        public async Task DeleteRoleFromDatabase()
        {
            var repository = await CreateRpeository();
            var expectedRole = Fix.Create<GuildRole>();

            var roles =
                (from _1 in repository.InsertRole(expectedRole)
                    from _2 in repository.DeleteRole(expectedRole)
                    from roleList in repository.GetRoles(expectedRole.GuildId)
                    select roleList).Right();

            Assert.Empty(roles);
        }

        private async Task<IRolesRepository> CreateRpeository([CallerMemberName] string? databaseName = null)
        {
            var context = await CreateContext(databaseName);
            return new RolesRepository(context);
        }
    }
}