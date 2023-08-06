using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Roles.Runners;

namespace OpenttdDiscord.Infrastructure.Tests.Roles.Runners
{
    public class GetRoleRunnerShould
    {
        private readonly IAkkaService akkaServiceSub = Substitute.For<IAkkaService>();

        private readonly IGetRoleLevelUseCase getRoleLevelUseCaseSub = Substitute.For<IGetRoleLevelUseCase>();

        private readonly GetRoleRunner sut;

        public GetRoleRunnerShould()
        {
            sut = new(
                akkaServiceSub,
                getRoleLevelUseCaseSub);
        }
    }
}
