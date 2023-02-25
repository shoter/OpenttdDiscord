using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public static class TransactionLogExtensions
    {
        public static EitherAsync<IError, T> LeftRollback<T>(this EitherAsync<IError, T> either, TransactionLog log)
        {
            return either.MapLeftAsync(async err =>
            {
                await log.Rollback();
                return err;
            });
        }
    }
}
