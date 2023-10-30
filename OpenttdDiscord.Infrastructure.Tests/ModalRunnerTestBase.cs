using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public class ModalRunnerTestBase : RunnerTestBase<IModalInteraction, ModalRunnerTestBase>
    {
        public async Task<IModalInteraction> Run(IOttdModalRunner runner)
        {
            var response = (await runner.Run(InteractionStub)).Right();
            await response.Execute(InteractionStub);

            return InteractionStub;
        }

        public EitherAsync<IError, IModalInteraction> RunExt(IOttdModalRunner runner) =>
            from response in runner.Run(InteractionStub)
            from result in response.Execute(InteractionStub)
            select InteractionStub;

        public EitherAsync<IError, IModalInteraction> NotExecuteFor(
            IOttdModalRunner runner,
            UserLevel userLevel)
        {
            var either =
                WithGuildUser()
                    .WithUserLevel(userLevel)
                    .RunExt(runner);
            either.IfRight(_ => Assert.Fail("Wrong user level"));
            either.IfLeft(err => Assert.True(err is IncorrectUserLevelError));
            return either;
        }
    }
}
