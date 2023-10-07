using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Roles.Runners;
using OpenttdDiscord.Infrastructure.Roles.UseCases;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class DeleteRoleRunnerShould : RunnerTestBase
    {
        private readonly DeleteRoleRunner sut;

        private readonly IDeleteRoleLevelUseCase deleteRoleLevelUseCaseSub = Substitute.For<IDeleteRoleLevelUseCase>();

        public DeleteRoleRunnerShould()
        {
            sut = new(
                akkaServiceSub,
                getRoleLevelUseCaseSub,
                deleteRoleLevelUseCaseSub);
        }


    }
}