using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Testing.Database
{
    public interface IContainerizedMysqlDatabase : IDisposable
    {
        int Port { get; }

        Task Start(string containerName);

        string GetConnectionString();
    }
}
