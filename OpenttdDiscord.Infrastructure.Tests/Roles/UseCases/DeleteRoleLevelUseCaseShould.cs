using AutoFixture;
using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Infrastructure.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.UseCases
{
    public class DeleteRoleLevelUseCaseShould
    {
        private readonly Fixture fix = new();

        private readonly DeleteRoleLevelUseCase sut;

        private readonly IRolesRepository rolesRepository = Substitute.For<IRolesRepository>();

        public DeleteRoleLevelUseCaseShould()
        {
            sut = new(rolesRepository);
            rolesRepository
                .DeleteRole(
                    default,
                    default)
                .ReturnsForAnyArgs(EitherAsyncUnit.Right(Unit.Default));
        }

        [Fact]
        public async Task DeleteCorrectRoleFromDatabase()
        {
            ulong guildId = fix.Create<ulong>();
            ulong roleId = fix.Create<ulong>();

            var result = await sut.Execute(
                guildId,
                roleId);
            Assert.True(result.IsRight);

            await rolesRepository
                .Received()
                .DeleteRole(
                    guildId,
                    roleId);
        }
    }
}