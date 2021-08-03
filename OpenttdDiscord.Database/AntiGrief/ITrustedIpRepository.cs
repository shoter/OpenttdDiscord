using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.AntiGrief
{
    public interface ITrustedIpRepository
    {
        Task<TrustedIp> Add(TrustedIp trustedIp);

        Task<TrustedIp> Get(string ipAddress);

        Task<List<TrustedIp>> GetAll();

        Task Remove(TrustedIp trustedIp);
    }
}
