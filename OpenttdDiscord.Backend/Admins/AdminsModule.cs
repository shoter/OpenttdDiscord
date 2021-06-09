﻿using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Admins
{
    public class AdminsModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<IAdminPortClientProvider, AdminPortClientProvider>();
            services.AddSingleton<IAdminPortClientFactory, AdminPortClientFactory>();
        }
    }
}
