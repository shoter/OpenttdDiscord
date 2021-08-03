using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenttdDiscord.Database.AntiGrief;

namespace OpenttdDiscord.Database.AntiGrief
{
    public interface ITrustedIpService
    {
        Task<TrustedIp> Add(TrustedIp trustedIp);

        Task<TrustedIp> Get(string ipAddress);

        Task<List<TrustedIp>> GetAll();

        Task Remove(TrustedIp trustedIp);
    }
}
