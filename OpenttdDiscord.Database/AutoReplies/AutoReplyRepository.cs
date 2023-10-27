using LanguageExt;
using OpenttdDiscord.Domain.AutoReplies;

namespace OpenttdDiscord.Database.AutoReplies
{
    internal class AutoReplyRepository : IAutoReplyRepository
    {
        private OttdContext DB { get; }

        public AutoReplyRepository(OttdContext dB)
        {
            DB = dB;
        }

        public EitherAsyncUnit InsertWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage)
        {
            throw new NotImplementedException();
        }

        public EitherAsync<IError, WelcomeMessage> GetWelcomeMessage(
            ulong guildId,
            Guid serverId)
        {
            throw new NotImplementedException();
        }

        public EitherAsyncUnit UpdateWelcomeMessage(
            ulong guildId,
            WelcomeMessage welcomeMessage)
        {
            throw new NotImplementedException();
        }
    }
}
