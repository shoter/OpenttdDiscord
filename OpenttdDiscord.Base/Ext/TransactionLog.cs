using LanguageExt;

namespace OpenttdDiscord.Base.Ext
{
    public class TransactionLog
    {
        private List<Func<EitherAsyncUnit>> rollbacks = new();

        public EitherUnit AddTransactionRollback(Func<EitherAsyncUnit> rollback)
        {
            rollbacks.Add(rollback);
            return Unit.Default;
        }

        public EitherAsyncUnit Rollback()
        {
            EitherAsyncUnit result = EitherAsyncUnit.Right(Unit.Default);

            foreach (var rollback in rollbacks)
            {
                result = result.Apply(_ => rollback());
            }

            return result;
        }
    }
}
