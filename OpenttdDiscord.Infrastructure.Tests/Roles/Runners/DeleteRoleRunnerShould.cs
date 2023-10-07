using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class DeleteRoleRunnerShould : RunnerTestBase
    {
        private readonly DeleteRoleRunner sut;

        private readonly IDeleteRoleLevelUseCase deleteRoleLevelUseCaseSub = Substitute.For<IDeleteRoleLevelUseCase>();

        private readonly ulong roleId;

        public DeleteRoleRunnerShould()
        {
            roleId = fix.Create<ulong>();
            WithOption(
                "role-id",
                roleId);

            sut = new(
                AkkaServiceSub,
                GetRoleLevelUseCaseSub,
                deleteRoleLevelUseCaseSub);
        }

        [Theory]
        [InlineData(UserLevel.User)]
        [InlineData(UserLevel.Moderator)]
        public async Task NotExecuteForNonAdmin(UserLevel userLevel)
        {
            await
                NotExecuteFor(
                    sut,
                    userLevel);
        }

        [Fact]
        public async Task DeleteRoleFromDatabase()
        {
            await WithGuildUser()
                .WithUserLevel(UserLevel.Admin)
                .RunExt(sut);

            await deleteRoleLevelUseCaseSub
                .Received()
                .Execute(
                    GuildId,
                    roleId);
        }
    }
}