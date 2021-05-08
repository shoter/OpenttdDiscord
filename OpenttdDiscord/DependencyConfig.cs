﻿using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Chatting;
using OpenttdDiscord.Commands;
using OpenttdDiscord.Configuration;
using OpenttdDiscord.Messaging;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Common;
using OpenttdDiscord.Admins;
using OpenttdDiscord.Reporting;

namespace OpenttdDiscord
{
    public static class DependencyConfig
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        public static void Init(IConfiguration config)
        {
            var services = new ServiceCollection();

            new ConfigModule().Register(services);
            new DatabaseModule().Register(services);
            new CommandsModule().Register(services);
            new MessagingModule().Register(services);
            new OttdModule().Register(services);
            new BackendModule().Register(services);
            new AdminModule().Register(services);

            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<IReportService, ReportService>();
            services.AddSingleton<ServerInfoProcessor>();
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddScoped<IPrivateMessageHandlingService, PrivateMessageHandlingService>();
            services.AddLogging(loggingBuilder =>
              {
                  // configure Logging with NLog
                  loggingBuilder.ClearProviders();
                  loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                  loggingBuilder.AddNLog(config);
              });
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
