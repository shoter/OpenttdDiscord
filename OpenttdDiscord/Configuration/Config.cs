using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Configuration
{
    public class Config
    {
        public MySqlConfig MySql { get; }

        public DiscordConfig Discord { get; }

        public Config()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("Configuration.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            this.MySql = new MySqlConfig()
            {
                Host = configuration["mysql:host"],
                Login = configuration["mysql:login"],
                Password = configuration["mysql:password"],
                Port = uint.Parse(configuration["mysql:port"]),
            };

            this.Discord = new DiscordConfig(configuration);

        }

    }
}
