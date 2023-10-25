using Akka.Actor;
using OpenttdDiscord.Infrastructure.Akkas;
using Xunit.Abstractions;

namespace OpenttdDiscord.Infrastructure.Tests.Akkas
{
    public class AkkaServiceShould : BaseActorTestKit
    {
        private readonly AkkaService akkaService;

        public AkkaServiceShould(ITestOutputHelper helper)
            : base(helper)
        {
            akkaService = new(Sys);
            akkaService.NotifyAboutAkkaStart();
        }

        [Fact]
        public async Task SelectActor()
        {
            // Arrange
            var testProbe = CreateTestProbe("2137");

            // Act
            var selection = (await akkaService.SelectActor("/system/2137")).Right();

            // Assert
            selection.Tell("2137");
            testProbe.ExpectMsg<string>("2137");
        }

        [Fact]
        public async Task SelectActor_AndSendItAMessage()
        {
            // Arrange
            var testProbe = CreateTestProbe("2137");
            string response = "JeszczeJak";

            // Act
            var askTask = akkaService.SelectAndAsk<string>(
                "/system/2137",
                "2137");

            // Assert
            testProbe.ExpectMsg<string>("2137");
            testProbe.Sender.Tell(response);

            Assert.Equal(
                response,
                (await askTask).Right());
        }
    }
}