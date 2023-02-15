using Microsoft.Extensions.Hosting;

namespace OpenttdDiscord.Discord
{
    internal class DiscordService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("I am hosted service which was run :)");
            return Task.CompletedTask;
        }
    }
}
