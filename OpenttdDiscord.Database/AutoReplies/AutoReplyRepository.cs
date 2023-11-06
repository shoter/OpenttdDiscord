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
            WelcomeMessage welcomeMessage) => from option in db.WelcomeMessages.FirstOptionalExt(
                w =>
                    w.GuildId == guildId &&
                    w.ServerId == welcomeMessage.ServerId)
            from _1 in Upsert(
                option,
                guildId,
                welcomeMessage)
            from _2 in db.SaveChangesAsyncExt()
            select Unit.Default;

        private EitherAsyncUnit Upsert(
            Option<WelcomeMessageEntity> welcomeMessageEntity,
            ulong guildId,
            WelcomeMessage newMessage) => welcomeMessageEntity.Match(
            some =>
            {
                some.Content = newMessage.Content;
                return Unit.Default;
            },
            () => db.WelcomeMessages.AddAsyncExt(
                new WelcomeMessageEntity(
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

        public EitherAsyncUnit UpsertAutoReply(
            ulong guildId,
            Guid serverId,
            AutoReply autoReply) => from option in db.AutoReplies.FirstOptionalExt(
                ar =>
                    ar.GuildId == guildId &&
                    ar.ServerId == serverId &&
                    ar.TriggerMessage == autoReply.TriggerMessage)
            from _1 in Upsert(
                option,
                guildId,
                serverId,
                autoReply)
            from _2 in db.SaveChangesAsyncExt()
            select Unit.Default;

        private EitherAsyncUnit Upsert(
            Option<AutoReplyEntity> autoReplyEntity,
            ulong guildId,
            Guid serverId,
            AutoReply newAutoReply) => autoReplyEntity.Match(
            some =>
            {
                some.TriggerMessage = newAutoReply.TriggerMessage;
                some.ResponseMessage = newAutoReply.ResponseMessage;
                some.AdditionalAction = newAutoReply.AdditionalAction.ToString();
                return Unit.Default;
            },
            () => db.AutoReplies.AddAsyncExt(
                new AutoReplyEntity(
                    guildId,
                    serverId,
                    newAutoReply.TriggerMessage,
                    newAutoReply.ResponseMessage,
                    newAutoReply.AdditionalAction.ToString())));

        public EitherAsync<IError, IReadOnlyCollection<AutoReply>> GetAutoReplies(
            ulong guildId,
            Guid serverId) => from servers in db.AutoReplies.WhereExt(
                ar =>
                    ar.GuildId == guildId &&
                    ar.ServerId == serverId)
            from serverList in servers.ToListExt()
            select serverList
                .Select(x => x.ToDomain())
                .ToList() as IReadOnlyCollection<AutoReply>;
    }
}