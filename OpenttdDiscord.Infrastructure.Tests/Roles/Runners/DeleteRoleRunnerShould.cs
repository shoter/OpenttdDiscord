using Discord;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class DeleteRoleRunnerShould : CommandRunnerTestBase
    {
        private readonly DeleteRoleRunner sut;

        private readonly IDeleteRoleLevelUseCase deleteRoleLevelUseCaseSub = Substitute.For<IDeleteRoleLevelUseCase>();

        private readonly IRole role;

        public DeleteRoleRunnerShould()
        {
            role = Substitute.For<IRole>();

            WithOption(
                "role",
                role);

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
                    role.Id);
        }
    }
}