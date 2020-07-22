using OpenTTDAdminPort;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Admins
{
    public class AdminClientRegisterInfo
    {
        public AdminClientRegisterInfo(Server server, IAdminPortClient client)
        {
            this.Server = server;
            this.Client = client;
        }

        public void AddUser(IAdminPortClientUser user)
        {
            RegisteredUsers.TryAdd(user, true);
        }

        public void RemoveUser(IAdminPortClientUser user)
        {
            RegisteredUsers.TryRemove(user, out _);
        }

        public bool HasAnyUsers() => RegisteredUsers.Count != 0;

        public bool IsRegistered(IAdminPortClientUser user) => RegisteredUsers.ContainsKey(user);

        public IEnumerable<IAdminPortClientUser> GetRegisteredUsers() => RegisteredUsers.Keys;

        public Server Server { get; }

        public IAdminPortClient Client { get; }

        private ConcurrentDictionary<IAdminPortClientUser, bool> RegisteredUsers { get; } = new ConcurrentDictionary<IAdminPortClientUser, bool>();

        public string GetKey() => $"{Server.GetUniqueKey()}";

    }
}
