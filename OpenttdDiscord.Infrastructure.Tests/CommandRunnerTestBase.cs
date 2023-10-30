using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public class CommandRunnerTestBase : RunnerTestBase<ISlashCommandInteraction, CommandRunnerTestBase>
    {
        public async Task<ISlashCommandInteraction> Run(IOttdSlashCommandRunner commandRunner)
        {
            var response = (await commandRunner.Run(CommandInteractionSub)).Right();
            await response.Execute(CommandInteractionSub);

            return CommandInteractionSub;
        }

        public EitherAsync<IError, ISlashCommandInteraction> RunExt(IOttdSlashCommandRunner commandRunner) =>
            from response in commandRunner.Run(CommandInteractionSub)
            from result in response.Execute(CommandInteractionSub)
            select CommandInteractionSub;

        public EitherAsync<IError, ISlashCommandInteraction> NotExecuteFor(
            IOttdSlashCommandRunner runner,
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