using System.Diagnostics.CodeAnalysis;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Base.Fundamentals;

namespace OpenttdDiscord.Infrastructure.Akkas
{
    public static class IAkkaServiceExtensions
    {
        [SuppressMessage("Member Design",
                         "AV1115:Member or local function contains the word \'and\', which suggests doing multiple things",
                         Justification = "It is much easier to mock this method in tests. It does 2 things which is wrong, however testing ease is tremendous help.")]
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