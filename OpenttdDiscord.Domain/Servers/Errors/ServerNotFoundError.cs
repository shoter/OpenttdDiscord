namespace OpenttdDiscord.Domain.Servers.Errors
{
    public class ServerNotFoundError : HumanReadableError
    {
        public ServerNotFoundError()
        : base("Server not found")
        {
        }
    }
}
