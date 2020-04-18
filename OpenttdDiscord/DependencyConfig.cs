using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using OpenttdDiscord.Backend;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Chatting;
using OpenttdDiscord.Commands;
using OpenttdDiscord.Configuration;
using OpenttdDiscord.Messaging;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.Tcp;
using OpenttdDiscord.Openttd.Network.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord
{
    public static class DependencyConfig
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        public static void Init(IConfiguration config)
        {
            var services = new ServiceCollection();

            new ConfigModule().Register(services);
            new BackendModule().Register(services);
            new CommandsModule().Register(services);
            new MessagingModule().Register(services);
            new OttdModule().Register(services);

            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddSingleton<ServerInfoProcessor>();
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
