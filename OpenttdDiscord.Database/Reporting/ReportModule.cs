using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Reporting
{
    public class ReportModule : IModule
    {
        public void Register(in IServiceCollection services)
        {
            services.AddSingleton<IReportServerRepository, ReportServerRepository>();
            services.AddSingleton<IReportServerService, ReportServerService>();
        }
    }
}
