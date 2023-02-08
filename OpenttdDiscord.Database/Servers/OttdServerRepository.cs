using LanguageExt;
using LanguageExt.Common;
using OpenttdDiscord.Database.Ottd.Servers;
using OpenttdDiscord.Domain.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    internal class OttdServerRepository : IOttdServerRepository
    {
        private OttdContext Db { get; } 

        public OttdServerRepository(OttdContext dbContext)
        {
            this.Db = dbContext;
        }

        public async Task<Result<Unit>> DeleteServer(Guid serverId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Unit>> InsertServer(OttdServer server)
        {
            return await new TryAsync<Unit>(async () =>
            {
                await Db.AddAsync<OttdServerEntity>(new(server));
                return Unit.Default;
            })();
        }

        public Task<Result<Unit>> UpdateServer(OttdServer server)
        {
            throw new NotImplementedException();
        }
    }
}
