using LanguageExt;
using OpenttdDiscord.Domain.AutoReplies;

namespace OpenttdDiscord.Database.AutoReplies
{
    internal class AutoReplyRepository : IAutoReplyRepository
    {
        private readonly OttdContext db;

        public AutoReplyRepository(OttdContext dB)
        {
            db = dB;
        }

        public EitherAsyncUnit UpsertWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage) =>
            from option in db.WelcomeMessages.FirstOptionalExt(w =>
                                                                   w.GuildId == guildId &&
                                                                   w.ServerId == welcomeMessage.ServerId)
            from _1 in Upsert(option, guildId, welcomeMessage)
            from _2 in db.SaveChangesAsyncExt()
            select Unit.Default;

        private EitherAsyncUnit Upsert(Option<WelcomeMessageEntity> welcomeMessageEntity, ulong guildId, WelcomeMessage newMessage)
             => welcomeMessageEntity.Match(
                some =>
                {
                    some.Content = newMessage.Content;
                    return Unit.Default;
                },
                () => db.WelcomeMessages.AddAsyncExt(new WelcomeMessageEntity(
                                                           guildId,
                                                           newMessage.ServerId,
                                                           newMessage.Content)));

        public EitherAsync<IError, Option<WelcomeMessage>> GetWelcomeMessage(
            ulong guildId,
            Guid serverId) => from query in db.WelcomeMessages.WhereExt(
                x => x.ServerId == serverId &&
                     x.GuildId == guildId)
            from result in query.FirstOptionalExt()
            select result.Map(x => x.ToDomain());
    }
}