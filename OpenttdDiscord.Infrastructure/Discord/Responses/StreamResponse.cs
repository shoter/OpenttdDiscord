using Discord;

namespace OpenttdDiscord.Infrastructure.Discord.CommandResponses
{
    internal class StreamResponse : InteractionResponseBase, IDisposable
    {
        private readonly Stream stream;
        private readonly string fileName;
        private readonly bool dispose;
        private bool disposedValue;

        public StreamResponse(Stream stream, string fileName, bool dispose = true)
        {
            this.stream = stream;
            this.fileName = fileName;
            this.dispose = dispose;
        }

        protected override async Task InternalExecute(ISlashCommandInteraction command)
        {
            await command.RespondWithFileAsync(stream, fileName);
            if(dispose)
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stream.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
