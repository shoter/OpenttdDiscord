using OpenttdDiscord.Backend;
using OpenttdDiscord.Database;
using OpenttdDiscord.Testing.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Testing
{
    public class MysqlTest : IDisposable
    {
        private readonly IContainerizedMysqlDatabase mysql = new ContainerizedMysqlDatabase();
        private bool started = false;
        private Mutex startMutex = new Mutex();

        public void Dispose()
        {
            mysql.Dispose();
        }

        public MySqlConfig GetMysql([CallerMemberName] string methodName = null)
        {
            if (started == false)
            {
                lock(startMutex)
                {
                    if (started == false)
                    {
                        mysql.Start($"openttd_{this.GetType().Name}_{methodName}").Wait();
                    }
                }
            }

            return new MySqlConfig()
            {
                ConnectionString = mysql.GetConnectionString()
            };
        }


    }
}
