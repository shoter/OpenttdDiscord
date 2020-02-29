using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Configuration
{
    public class ConfigModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
        .AddJsonFile("Configuration.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();

            services.AddSingleton(new MySqlConfig()
            {
                ConnectionString = configuration["mysql:connectionString"],
            });

            services.AddSingleton(new OpenttdDiscordConfig(configuration));
        }
    }
}
