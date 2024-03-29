﻿using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenttdDiscord.Database;
using OpenttdDiscord.Discord.Options;
using OpenttdDiscord.Discord.Services;
using OpenttdDiscord.Infrastructure;
using Serilog;

namespace OpenttdDiscord.Discord
{
    public static class ApplicationBuilder
    {
        [ExcludeFromCodeCoverage]
        public static IHostBuilder CreateHostBuilder()
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureLogging()
                .RegisterDependencies();
        }

        [ExcludeFromCodeCoverage]
        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog(
                (
                    context,
                    configuration) =>
                {
                    configuration.MinimumLevel.Verbose()
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext();
                });
        }

        [ExcludeFromCodeCoverage]
        public static IHostBuilder RegisterDependencies(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices(
                (
                    HostBuilderContext context,
                    IServiceCollection services) =>
                {
                    var discordClient = new DiscordSocketClient(
                        new()
                        {
                            GatewayIntents = GatewayIntents.MessageContent | GatewayIntents.AllUnprivileged,
                            UseInteractionSnowflakeDate = false,
                        });
                    services
                        .AddSingleton(ActorSystem.Create("OttdDiscord"))
                        .AddSingleton(discordClient)
                        .AddSingleton<IDiscordClient>(discordClient)
                        .RegisterModules()
                        .AddHostedServices()
                        .ConfigureOptions(context);
                });
        }

        [ExcludeFromCodeCoverage]
        public static IServiceCollection AddHostedServices(this IServiceCollection services)
        {
            return services
                .AddHostedService<DiscordService>()
                .AddHostedService<AkkaStarterService>();
        }

        [ExcludeFromCodeCoverage]
        public static IServiceCollection ConfigureOptions(
            this IServiceCollection services,
            HostBuilderContext context)
        {
            return services
                .Configure<DiscordOptions>(context.Configuration.GetSection("Discord"))
                .Configure<DatabaseOptions>(context.Configuration.GetSection("Database"));
        }
    }
}