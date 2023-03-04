using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.DiscordRelated;
using OpenttdDiscord.Domain.Rcon;

namespace OpenttdDiscord.Database.Rcon
{
    internal class RconChannelRepository : IRconChannelRepository
    {
        private OttdContext DB { get; }

        public RconChannelRepository(OttdContext dB)
        {
            DB = dB;
        }

        public EitherAsyncUnit Delete(Guid serverId, ulong channelId)
            => TryAsync<EitherUnit>(async () =>
            {
                int deletedRows = await DB.RconChannels
                    .Where(cc => cc.ServerId == serverId && cc.ChannelId == channelId)
                    .DeleteFromQueryAsync();

                if (deletedRows == 0)
                {
                    return new HumanReadableError("No chat channel was found for deletion");
                }

                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, List<RconChannel>> GetRconChannelsForTheServer(Guid serverId)
            => TryAsync<Either<IError, List<RconChannel>>>(async () =>
            {
                return (await DB.RconChannels
                    .AsNoTracking()
                    .Where(cc => cc.ServerId == serverId)
                    .ToListAsync())
                    .Select(cc => cc.ToDomain())
                    .ToList();
            }).ToEitherAsyncErrorFlat();

        public EitherAsyncUnit Insert(RconChannel rconChannel)
            => TryAsync<EitherUnit>(async () =>
            {
                await DB.RconChannels.AddAsync(new(rconChannel));
                await DB.SaveChangesAsync();
                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, Option<RconChannel>> GetRconChannel(Guid serverId, ulong channelId)
              => TryAsync<Either<IError, Option<RconChannel>>>(async () =>
              {
                  var adminChannel = await DB.RconChannels
                      .AsNoTracking()
                      .FirstOrDefaultAsync(cc => cc.ServerId == serverId && cc.ChannelId == channelId);

                  if (adminChannel == null)
                  {
                      return Option<RconChannel>.None;
                  }

                  return Option<RconChannel>.Some(adminChannel.ToDomain());
              }).ToEitherAsyncErrorFlat();
    }
}
