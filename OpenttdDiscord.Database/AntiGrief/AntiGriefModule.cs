using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using OpenttdDiscord.Common;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class AntiGriefModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<ITrustedIpRepository, TrustedIpRepository>();
            services.AddSingleton<IAntiGriefRepository, AntiGriefRepository>();

            services.AddSingleton<ITrustedIpService, TrustedIpService>();
            services.AddSingleton<IAntiGriefService, AntiGriefService>();
        }
    }
}
