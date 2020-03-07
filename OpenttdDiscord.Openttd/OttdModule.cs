using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using OpenttdDiscord.Openttd.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Openttd
{
    public class OttdModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            new NetworkModule().Register(services);
            services.AddSingleton<IRevisionTranslator, RevisionTranslator>();
        }
    }
}
