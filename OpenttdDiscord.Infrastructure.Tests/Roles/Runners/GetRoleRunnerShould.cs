using Discord;
using NSubstitute;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Roles.Runners;
using Array = System.Array;

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

            getRoleLevelUseCaseSub.Execute(default!)
                .ReturnsForAnyArgs(UserLevel.Admin);
        }

        [Fact]
        public async Task ReturnTextCommandResponse_WithWordUser_ForNonGuildUser()
        {
            var commandSub = Substitute.For<ISlashCommandInteraction>();
            var data = Substitute.For<IApplicationCommandInteractionData>();
            IUser user = Substitute.For<IUser>();

            commandSub.Data.Returns(data);
            data.Options.Returns(Array.Empty<IApplicationCommandInteractionDataOption>());

            var response = (await sut.Run(commandSub)).Right();
            await response.Execute(commandSub);

            await commandSub.Received()
                .RespondAsync(Arg.Is<string>(txt => txt.Contains("user")));
        }
    }
}