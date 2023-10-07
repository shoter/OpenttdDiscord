using AutoFixture;
using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using Array = System.Array;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public class RunnerTestBase
    {
        protected readonly IAkkaService akkaServiceSub = Substitute.For<IAkkaService>();

        protected readonly IGetRoleLevelUseCase getRoleLevelUseCaseSub = Substitute.For<IGetRoleLevelUseCase>();

        protected readonly Fixture fix = new();

        protected IUser UserSub { get; private set; } = Substitute.For<IGuildUser>();

        protected ulong GuildId { get; private set; }

        protected ulong ChannelId { get; private set; }

        protected ISlashCommandInteraction CommandInteractionSub { get; } = Substitute.For<ISlashCommandInteraction>();

        private readonly IApplicationCommandInteractionData dataSub =
            Substitute.For<IApplicationCommandInteractionData>();

        private List<IApplicationCommandInteractionDataOption> options = new();

        public RunnerTestBase()
        {
            CommandInteractionSub.Data.Returns(dataSub);
            dataSub.Options.Returns(options);

            WithUserLevel(UserLevel.Admin)
                .WithGuildUser()
                .WithGuildId(fix.Create<ulong>())
                .WithChannelId(fix.Create<ulong>());
        }

        public RunnerTestBase WithGuildId(ulong guildId)
        {
            CommandInteractionSub.GuildId.Returns(guildId);
            GuildId = guildId;
            return this;
        }

        public RunnerTestBase WithChannelId(ulong channelId)
        {
            CommandInteractionSub.ChannelId.Returns(channelId);
            ChannelId = channelId;
            return this;
        }

        public RunnerTestBase WithUser(IUser newUser)
        {
            UserSub = newUser;
            CommandInteractionSub.User.Returns(newUser);
            return this;
        }

        public RunnerTestBase WithGuildUser() => WithUser(Substitute.For<IGuildUser>());

        public IGuildUser WithGuildUserReturn()
        {
            IGuildUser guildUserSub = Substitute.For<IGuildUser>();
            guildUserSub.Id.Returns(fix.Create<ulong>());
            WithUser(guildUserSub);
            return guildUserSub;
        }

        public RunnerTestBase WithNonGuildUser() => WithUser(Substitute.For<IUser>());

        public RunnerTestBase WithUserLevel(UserLevel userLevel)
        {
            getRoleLevelUseCaseSub.Execute(default!)
                .ReturnsForAnyArgs(userLevel);
            return this;
        }

        public RunnerTestBase WithOption(
            string name,
            object value,
            ApplicationCommandOptionType type = ApplicationCommandOptionType.String)
        {
            var option = Substitute.For<IApplicationCommandInteractionDataOption>();
            option.Name.Returns(name);
            option.Value.Returns(value);
            option.Type.Returns(type);
            option.Options.Returns(Array.Empty<IApplicationCommandInteractionDataOption>());
            options.Add(option);
            return this;
        }

        public RunnerTestBase WithOption(
            string name,
            int value) => WithOption(
            name,
            value,
            ApplicationCommandOptionType.Integer);

        public RunnerTestBase WithOption(
            string name,
            long value) => WithOption(
            name,
            value,
            ApplicationCommandOptionType.Integer);

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

        protected EitherAsync<IError, ISlashCommandInteraction> NotExecuteFor(IOttdSlashCommandRunner runner,
                                           UserLevel userLevel)
        {
            var either = RunExt(runner);
            either.IfRight(_ => Assert.Fail("Wrong user level"));
            either.IfLeft(err => Assert.True(err is IncorrectUserLevelError));
            return either;
        }
    }
}