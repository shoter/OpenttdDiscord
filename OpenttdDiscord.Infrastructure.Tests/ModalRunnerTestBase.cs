using Discord;
using OpenttdDiscord.Domain.Roles.Errors;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Infrastructure.Discord.CommandRunners;
using OpenttdDiscord.Infrastructure.Discord.ModalRunners;

namespace OpenttdDiscord.Infrastructure.Tests
{
    public class ModalRunnerTestBase : RunnerTestBase<IModalInteraction, ModalRunnerTestBase>
    {
        private readonly IModalInteractionData interactionDataSub = Substitute.For<IModalInteractionData>();
        private readonly Dictionary<string, IComponentInteractionData> components = new();

        public ModalRunnerTestBase()
        {
            this.InteractionStub.Data.Returns(interactionDataSub);
            interactionDataSub.Components.Returns(_ => components.Values);
        }

        public async Task<IModalInteraction> Run(IOttdModalRunner runner)
        {
            var response = (await runner.Run(InteractionStub)).Right();
            await response.Execute(InteractionStub);

            return InteractionStub;
        }

        public ModalRunnerTestBase WithTextInput(
            string id,
            object value)
        {
            var component = Substitute.For<IComponentInteractionData>();
            component.CustomId.Returns(id);
            component.Value.Returns(value);
            component.Type.Returns(ComponentType.TextInput);
            components[id] = component;
            return this;
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