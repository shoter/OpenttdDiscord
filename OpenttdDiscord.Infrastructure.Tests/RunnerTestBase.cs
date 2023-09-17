using AutoFixture;
using Discord;
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

        protected ulong GuildId { get; }

        protected ISlashCommandInteraction CommandInteractionSub { get; } = Substitute.For<ISlashCommandInteraction>();

        private readonly IApplicationCommandInteractionData dataSub =
            Substitute.For<IApplicationCommandInteractionData>();

        private List<IApplicationCommandInteractionDataOption> options = new();

        public RunnerTestBase()
        {
            GuildId = fix.Create<ulong>();
            CommandInteractionSub.Data.Returns(dataSub);
            dataSub.Options.Returns(options);

            WithUserLevel(UserLevel.Admin)
                .WithGuildUser()
                .WithGuildId(GuildId);
        }

        public RunnerTestBase WithGuildId(ulong guildId)
        {
            CommandInteractionSub.GuildId.Returns(guildId);
            return this;
        }

        public RunnerTestBase WithUser(IUser newUser)
        {
            UserSub = newUser;
            CommandInteractionSub.User.Returns(newUser);
            return this;
        }

        public RunnerTestBase WithGuildUser() => WithUser(Substitute.For<IGuildUser>());

        public RunnerTestBase WithNonGuildUser() => WithUser(Substitute.For<IUser>());

        public RunnerTestBase WithUserLevel(UserLevel userLevel)
        {
            getRoleLevelUseCaseSub.Execute(default!)
                .ReturnsForAnyArgs(userLevel);
            return this;
        }

        public RunnerTestBase WithOption(
            string name,
            string value,
            ApplicationCommandOptionType type)
        {
            var option = Substitute.For<IApplicationCommandInteractionDataOption>();
            option.Name.Returns(name);
            option.Value.Returns(value);
            option.Type.Returns(type);
            option.Options.Returns(Array.Empty<IApplicationCommandInteractionDataOption>());
            options.Add(option);
            return this;
        }

        public async Task<ISlashCommandInteraction> Run(IOttdSlashCommandRunner commandRunner)
        {
            var response = (await commandRunner.Run(CommandInteractionSub)).Right();
            await response.Execute(CommandInteractionSub);

            return CommandInteractionSub;
        }
    }
}