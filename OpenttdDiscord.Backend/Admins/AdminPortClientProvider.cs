using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Openttd;
using OpenttdDiscord.Openttd.Network.AdminPort;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Backend.Admins
{
    public class AdminPortClientProvider : IAdminPortClientProvider
    {
        private IServerService serverService;

        private IAdminPortClientFactory adminPortClientFactory;

        private ConcurrentDictionary<string, AdminClientRegisterInfo> RegisterInfo { get; } = new ConcurrentDictionary<string, AdminClientRegisterInfo>();

        public AdminPortClientProvider(IServerService serverService, IAdminPortClientFactory adminPortClientFactory)
        {
            this.serverService = serverService;
            this.adminPortClientFactory = adminPortClientFactory;

            this.serverService.PasswordChanged += ServerService_PasswordChanged;
        }

        private void ServerService_PasswordChanged(object sender, Server server)
        {
            AdminClientRegisterInfo newInfo = new AdminClientRegisterInfo(server, adminPortClientFactory.Create(new ServerInfo(
                server.ServerIp, server.ServerPort, server.ServerPassword)));

            this.RegisterInfo.AddOrUpdate(server.GetUniqueKey(),
                (_) => newInfo, (_, old) =>
                {
                    foreach (var user in old.GetRegisteredUsers())
                        newInfo.AddUser(user);

                    return newInfo;
                });
        }

        public IAdminPortClient GetClient(object owner, Server server)
        {
            if (RegisterInfo.TryGetValue(server.GetUniqueKey(), out AdminClientRegisterInfo info))
            {
                if (info.IsRegistered(owner))
                {
                    return info.Client;
                }
            }
            throw new OttdException("Client is not registered - therefore it cannot retrieve AdminPortClient");
        }

        public async Task Register(object owner, Server server)
        {
            AdminClientRegisterInfo info = RegisterInfo.GetOrAdd(server.GetUniqueKey(), (_) =>
             {
                 IAdminPortClient client = this.adminPortClientFactory.Create(
                    new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword)
                    );

                 return new AdminClientRegisterInfo(server, client);
             });

            if (info.Client.ConnectionState == AdminConnectionState.Idle)
                await info.Client.Join();

            info.AddUser(owner);
        }

        public async Task Unregister(object owner, Server server)
        {
            if (RegisterInfo.TryGetValue(server.GetUniqueKey(), out AdminClientRegisterInfo info))
            {
                info.RemoveUser(owner);
                if (info.HasAnyUsers() == false && info.Client.ConnectionState != AdminConnectionState.Idle)
                {
                    await info.Client.Disconnect();
                }
            }
        }

        public bool IsRegistered(object owner, Server server)
        {
            if (RegisterInfo.TryGetValue(server.GetUniqueKey(), out AdminClientRegisterInfo info))
            {
                return info.IsRegistered(owner);
            }
            return false;
        }
    }
}