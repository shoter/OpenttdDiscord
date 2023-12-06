using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Base.Fundamentals;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public static class IAkkaServiceExtensions
    {
        public static EitherAsync<IError, TExpectedMessage> SelectAndAsk<TExpectedMessage>(
            this IAkkaService akkaService,
            string path,
            object message,
            TimeSpan? timeout = null) => from result in akkaService.SelectAndAsk(
                path,
                message,
                timeout)
            from convertedResult in result.ConvertExt<TExpectedMessage>()
            select convertedResult;
    }
}