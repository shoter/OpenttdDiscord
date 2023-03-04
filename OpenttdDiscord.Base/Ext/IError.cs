using Microsoft.Extensions.Logging;

namespace OpenttdDiscord.Base.Ext
{
    public interface IError
    {
        public string Reason { get; }

        void LogError(ILogger logger) => logger.LogError(Reason);
    }
}
