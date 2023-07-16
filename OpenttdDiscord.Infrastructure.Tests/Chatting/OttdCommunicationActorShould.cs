using Akka.Actor;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using OpenTTDAdminPort;
using OpenttdDiscord.Domain.Chatting.Translating;
using OpenttdDiscord.Domain.Servers;
using OpenttdDiscord.Infrastructure.Akkas;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.Chatting
{
    public class OttdCommunicationActorShould : BaseActorTestKit
    {
        private readonly IAkkaService akkaServiceMock = Substitute.For<IAkkaService>();
        private readonly IChatTranslator chatTranslatorMock = Substitute.For<IChatTranslator>();
        private readonly IAdminPortClient adminPortClientMock = Substitute.For<IAdminPortClient>();

        private readonly ulong channelId;
        private readonly OttdServer ottdServer;

        public OttdCommunicationActorShould(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
            channelId = fix.Create<ulong>();
            ottdServer = fix.Create<OttdServer>();

            // sut = ActorOf(DiscordCOmmu/**/
        }

        protected override void InitializeServiceProvider(IServiceCollection services)
        {
            base.InitializeServiceProvider(services);

            services.AddSingleton(akkaServiceMock);
        }

        [Fact]
        public void NotSendGameMessage_WhenChatIsNotOfBroadcastType()
        {
            var channelId = fix.Create<ulong>();
            var server = fix.Create<OttdServer>();



        }

    }
}