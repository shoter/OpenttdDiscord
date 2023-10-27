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

        public EitherAsyncUnit InsertWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage) => from _1 in db.WelcomeMessages.AddAsyncExt(
                new WelcomeMessageEntity(
                    guildId,
                    welcomeMessage.ServerId,
                    welcomeMessage.Content))
            from _2 in db.SaveChangesAsyncExt()
            select Unit.Default;

        public EitherAsync<IError, Option<WelcomeMessage>> GetWelcomeMessage(
            ulong guildId,
            Guid serverId) => from query in db.WelcomeMessages.WhereExt(
                x => x.ServerId == serverId &&
                     x.GuildId == guildId)
            from result in query.FirstOptionalExt()
            select result.Map(x => x.ToDomain());

        public EitherAsyncUnit UpdateWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage) => from query in db.WelcomeMessages.WhereExt(
                x => x.ServerId == welcomeMessage.ServerId &&
                     x.GuildId == guildId)
            from entity in query.FirstExt()
            from _1 in entity.Update(welcomeMessage.Content)
            from _2 in db.SaveChangesAsyncExt()
            select Unit.Default;
    }
}