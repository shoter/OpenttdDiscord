using System.Runtime.CompilerServices;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Database.Roles;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Domain.Security;
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

        [Fact]
        public async Task UpdateRoleInDatabase()
        {
            var repository = await CreateRpeository();
            var expectedRole = Fix.Create<GuildRole>() with { RoleLevel = UserLevel.Moderator };
            var updateRole = expectedRole with { RoleLevel = UserLevel.Admin };

            var roles =
                (from _1 in repository.InsertRole(expectedRole)
                    from _2 in repository.UpdateRole(updateRole)
                    from roleList in repository.GetRoles(expectedRole.GuildId)
                    select roleList).Right();

            var role = roles.Single();
            Assert.Equal(updateRole, role);
        }

        private async Task<IRolesRepository> CreateRpeository([CallerMemberName] string databaseName = "default")
        {
            var context = await CreateContext(databaseName);
            return new RolesRepository(context);
        }
    }
}