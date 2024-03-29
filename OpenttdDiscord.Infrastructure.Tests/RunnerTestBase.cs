using Discord;
using NSubstitute.Extensions;
using OpenttdDiscord.Domain.Roles.UseCases;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Akkas;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public class RunnerTestBase<TUserInteraction, TSelf>
        where TUserInteraction : class, IDiscordInteraction
        where TSelf : RunnerTestBase<TUserInteraction, TSelf>
    {
        protected readonly IAkkaService AkkaServiceSub = Substitute.For<IAkkaService>();

        protected readonly IGetRoleLevelUseCase GetRoleLevelUseCaseSub = Substitute.For<IGetRoleLevelUseCase>();

        protected readonly Fixture fix = new();

        protected IUser UserSub { get; private set; } = Substitute.For<IGuildUser>();

        protected ulong GuildId { get; private set; }

        protected ulong ChannelId { get; private set; }

        protected TUserInteraction InteractionStub { get; } = Substitute.For<TUserInteraction>();

        public RunnerTestBase()
        {
            WithUserLevel(UserLevel.Admin)
                .WithGuildUser()
                .WithGuildId(fix.Create<ulong>())
                .WithChannelId(fix.Create<ulong>());
        }

        public TSelf WithGuildId(ulong guildId)
        {
            InteractionStub.GuildId.Returns(guildId);
            GuildId = guildId;
            return (TSelf) this;
        }

        public TSelf WithChannelId(ulong channelId)
        {
            InteractionStub.ChannelId.Returns(channelId);
            ChannelId = channelId;
            return (TSelf) this;
        }

        public TSelf WithUser(IUser newUser)
        {
            UserSub = newUser;
            InteractionStub.User.Returns(newUser);
            return (TSelf) this;
        }

        public TSelf WithGuildUser() => WithUser(Substitute.For<IGuildUser>());

        public IGuildUser WithGuildUserReturn()
        {
            IGuildUser guildUserSub = Substitute.For<IGuildUser>();
            guildUserSub.Id.Returns(fix.Create<ulong>());
            WithUser(guildUserSub);
            return guildUserSub;
        }

        public TSelf WithNonGuildUser() => WithUser(Substitute.For<IUser>());

        public TSelf WithUserLevel(UserLevel userLevel)
        {
            GetRoleLevelUseCaseSub.Execute(default!)
                .ReturnsForAnyArgs(userLevel);
            return (TSelf) this;
        }
    }
}