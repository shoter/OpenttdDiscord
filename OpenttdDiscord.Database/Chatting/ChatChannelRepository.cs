using LanguageExt;
using LanguageExt.Pipes;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Chatting;

namespace OpenttdDiscord.Database.Chatting
{
    internal class ChatChannelRepository : IChatChannelRepository
    {
        private OttdContext DB { get; }

        public ChatChannelRepository(OttdContext dB)
        {
            DB = dB;
        }

        public EitherAsyncUnit Delete(Guid serverId, ulong channelId)
            => TryAsync<EitherUnit>(async () =>
            {
                int deletedRows = await DB.ChatChannels
                    .Where(cc => cc.ServerId == serverId && cc.ChannelId == channelId)
                    .DeleteFromQueryAsync();

                if (deletedRows == 0)
                {
                    return new HumanReadableError("No chat channel was found for deletion");
                }

                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, List<ChatChannel>> GetChatChannelsForServer(Guid serverId)
            => TryAsync<Either<IError, List<ChatChannel>>>(async () =>
               {
                   return (await DB.ChatChannels
                       .AsNoTracking()
                       .Where(cc => cc.ServerId == serverId)
                       .ToListAsync())
                       .Select(cc => cc.ToDomain())
                       .ToList();
               }).ToEitherAsyncErrorFlat();

        public EitherAsyncUnit Insert(ChatChannel chatChannel)
            => TryAsync<EitherUnit>(async () =>
            {
                await DB.ChatChannels.AddAsync(new(chatChannel));
                await DB.SaveChangesAsync();
                return Unit.Default;
            }).ToEitherAsyncErrorFlat();

        public EitherAsync<IError, Option<ChatChannel>> GetChatChannelForServer(Guid serverId, ulong channelId)
              => TryAsync<Either<IError, Option<ChatChannel>>>(async () =>
              {
                  var chatChannel = await DB.ChatChannels
                      .AsNoTracking()
                      .FirstOrDefaultAsync(cc => cc.ServerId == serverId && cc.ChannelId == channelId);

                  if (chatChannel == null)
                  {
                      return Option<ChatChannel>.None;
                  }

                  return Option<ChatChannel>.Some(chatChannel.ToDomain());
              }).ToEitherAsyncErrorFlat();
    }
}
