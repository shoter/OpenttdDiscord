using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenttdDiscord.Database.AntiGrief;

namespace OpenttdDiscord.Database.AntiGrief
{
    public class TrustedIpService : ITrustedIpService
    {
        private ITrustedIpRepository trustedIpRepository { get; }

        private ConcurrentDictionary<string, TrustedIp> Cache { get; set; } = new ConcurrentDictionary<string, TrustedIp>();

        public TrustedIpService(ITrustedIpRepository trustedIpRepository)
        {
            this.trustedIpRepository = trustedIpRepository;
        }

        public async Task<TrustedIp> Add(TrustedIp trustedIp)
        {
            TrustedIp ip = await trustedIpRepository.Add(trustedIp);
            Cache.AddOrUpdate(ip.IpAddress, ip, (key, val) => ip);
            return ip;
        }

        public async Task<List<TrustedIp>> GetAll()
        {

            List<TrustedIp> ips = await trustedIpRepository.GetAll();

            foreach(var ip in ips)
            {
                Cache.AddOrUpdate(ip.IpAddress, ip, (key, val) => ip);
            }
            return ips;
        }

        public async Task Remove(string trustedIp)
        {
            await trustedIpRepository.Remove(trustedIp);
            Cache.TryRemove(trustedIp, out _);
        }

        public async Task<TrustedIp> Get(string ipAddress)
        {
            if(Cache.TryGetValue(ipAddress, out TrustedIp ip))
            {
                return ip;
            }

            return await trustedIpRepository.Get(ipAddress);
        }

        public async Task<bool> Exists(string ipAddress) => await Get(ipAddress) != null;
    }
}
