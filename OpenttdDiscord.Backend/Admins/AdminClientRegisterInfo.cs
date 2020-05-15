using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd.Network.AdminPort;
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

        public void AddUser(object user)
        {
            RegisteredUsers.TryAdd(user, true);
        }

        public void RemoveUser(object user)
        {
            RegisteredUsers.TryRemove(user, out _);
        }

        public bool HasAnyUsers() => RegisteredUsers.Count != 0;

        public bool IsRegistered(object user) => RegisteredUsers.ContainsKey(user);

        public IEnumerable<object> GetRegisteredUsers() => RegisteredUsers.Keys;

        public Server Server { get; }

        public IAdminPortClient Client { get; }

        private ConcurrentDictionary<object, bool> RegisteredUsers { get; } = new ConcurrentDictionary<object, bool>();

        public string GetKey() => $"{Server.GetUniqueKey()}";

    }
}
