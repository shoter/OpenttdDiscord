namespace OpenttdDiscord.Tests.Common.Akkas
{
    /// <param name="IsBlocking">Will block all further handles.</param>
    /// <param name="DisposeAfterExpected">Will be removed after isExpectedMessage=true</param>
    public record TesterHandle(
        Func<object, bool> IsExpectedMessage,
        Func<object, object> CreateResponse,
        bool IsBlocking = false,
        bool DisposeAfterExpected = false);
}