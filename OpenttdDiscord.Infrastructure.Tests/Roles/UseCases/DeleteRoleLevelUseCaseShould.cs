using OpenttdDiscord.Domain.Roles;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Messages;
using OpenttdDiscord.Infrastructure.Roles.UseCases;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.UseCases
{
    public class DeleteRoleLevelUseCaseShould : BaseActorTestKit
    {
        private readonly DeleteRoleLevelUseCase sut;

        private readonly IRolesRepository rolesRepository = Substitute.For<IRolesRepository>();

        private readonly IAkkaService akkaService = Substitute.For<IAkkaService>();

        public DeleteRoleLevelUseCaseShould(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            sut = new(rolesRepository, akkaService);

            rolesRepository
                .DeleteRole(
                    default,
                    default)
                .ReturnsForAnyArgs(EitherAsyncUnit.Right(Unit.Default));

            akkaService
                .ReturnsActorOnSelect(
                    MainActors.Paths.Guilds,
                    probe);
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

            probe
                .ExpectMsg<DeleteRole>(
                    msg =>
                        msg.GuildId == guildId &&
                        msg.RoleId == roleId);
        }
    }
}