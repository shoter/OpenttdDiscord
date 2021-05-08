﻿using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Servers;
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
        private readonly IServerService serverService;

        private ConcurrentDictionary<string, AdminClientRegisterInfo> RegisterInfo { get; } = new ConcurrentDictionary<string, AdminClientRegisterInfo>();

        public AdminPortClientProvider(IServerService serverService)
        {
            this.serverService = serverService;

            this.serverService.PasswordChanged += ServerService_PasswordChanged;
        }

        private void ServerService_PasswordChanged(object sender, Server server)
        {
            var serverInfo = new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword);

            AdminClientRegisterInfo newInfo = new AdminClientRegisterInfo(server, new AdminPortClient(serverInfo));
            newInfo.Client.EventReceived += this.Client_EventReceived;

            this.RegisterInfo.AddOrUpdate(server.GetUniqueKey(),
                (_) => newInfo, (_, old) =>
                {
                    old.Client.EventReceived -= this.Client_EventReceived;
                    foreach (var user in old.GetRegisteredUsers())
                        newInfo.AddUser(user);

                    return newInfo;
                });
        }

        public IAdminPortClient GetClient(IAdminPortClientUser owner, Server server)
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

        public async Task Register(IAdminPortClientUser owner, Server server)
        {
            AdminClientRegisterInfo info = RegisterInfo.GetOrAdd(server.GetUniqueKey(), (_) =>
             {
                 var serverInfo = new ServerInfo(server.ServerIp, server.ServerPort, server.ServerPassword);
                 IAdminPortClient client = new AdminPortClient(serverInfo);
                 client.EventReceived += Client_EventReceived;

                 return new AdminClientRegisterInfo(server, client);
             });

            if (info.Client.ConnectionState == AdminConnectionState.Idle)
                await info.Client.Connect();

            info.AddUser(owner);
        }

        private void Client_EventReceived(object sender, IAdminEvent adminEvent)
        {
            IAdminPortClient client = sender as IAdminPortClient;

            List<(IEnumerable<IAdminPortClientUser> users, Server server)> serverUsers = new List<(IEnumerable<IAdminPortClientUser> users, Server server)>();

            var currentServerRegisterInfo = RegisterInfo.Values
                .Where(x => x.Server.ServerIp == client.ServerInfo.ServerIp &&
                            x.Server.ServerPort == client.ServerInfo.ServerPort);



            foreach (var info in currentServerRegisterInfo)
            {
                serverUsers.Add((info.GetRegisteredUsers(), info.Server));
            }

            serverUsers.ForEach(su =>
            {
                foreach(var u in su.users)
                {
                    u.ParseServerEvent(su.server, adminEvent);
                }
            });
        }

        public async Task Unregister(IAdminPortClientUser owner, Server server)
        {
            if (RegisterInfo.TryGetValue(server.GetUniqueKey(), out AdminClientRegisterInfo info))
            {
                info.RemoveUser(owner);
                if (info.HasAnyUsers() == false && info.Client.ConnectionState != AdminConnectionState.Idle)
                {
                    info.Client.EventReceived -= this.Client_EventReceived;
                    await info.Client.Disconnect();
                }
            }
        }

        public bool IsRegistered(IAdminPortClientUser owner, Server server)
        {
            if (RegisterInfo.TryGetValue(server.GetUniqueKey(), out AdminClientRegisterInfo info))
            {
                return info.IsRegistered(owner);
            }
            return false;
        }
    }
}