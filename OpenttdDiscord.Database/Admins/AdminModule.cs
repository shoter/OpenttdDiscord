using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Admins
{
    public class AdminModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton < IAdminChannelRepository,AdminChannelRepository>();
            services.AddSingleton<IAdminChannelService, AdminChannelService>();
        }
    }
}
