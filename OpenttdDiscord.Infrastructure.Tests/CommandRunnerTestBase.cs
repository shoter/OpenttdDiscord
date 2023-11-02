using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandResponses;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public class CommandRunnerTestBase : RunnerTestBase<ISlashCommandInteraction, CommandRunnerTestBase>
    {
        private readonly IApplicationCommandInteractionData dataSub =
            Substitute.For<IApplicationCommandInteractionData>();

        private readonly List<IApplicationCommandInteractionDataOption> options = new();

        protected CommandRunnerTestBase()
        {
            InteractionStub.Data.ReturnsForAnyArgs(dataSub);
            dataSub.Options.ReturnsForAnyArgs(options);
        }

        public CommandRunnerTestBase WithOption(
            string name,
            object value,
            ApplicationCommandOptionType type = ApplicationCommandOptionType.String)
        {
            var option = Substitute.For<IApplicationCommandInteractionDataOption>();
            option.Name.Returns(name);
            option.Value.Returns(value);
            option.Type.Returns(type);
            option.Options.Returns(System.Array.Empty<IApplicationCommandInteractionDataOption>());
            options.Add(option);
            return this;
        }

        public CommandRunnerTestBase WithOption(
            string name,
            int value) => WithOption(
            name,
            value,
            ApplicationCommandOptionType.Integer);

        public CommandRunnerTestBase WithOption(
            string name,
            long value) => WithOption(
            name,
            value,
            ApplicationCommandOptionType.Integer);

        public async Task<ISlashCommandInteraction> Run(IOttdSlashCommandRunner commandRunner)
        {
            var response = (await commandRunner.Run(InteractionStub)).Right();
            await response.Execute(InteractionStub);

            return InteractionStub;
        }

        public EitherAsync<IError, IInteractionResponse> RunExt(IOttdSlashCommandRunner commandRunner) =>
            from response in commandRunner.Run(InteractionStub)
            select response;

        public EitherAsync<IError, IInteractionResponse> NotExecuteFor(
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