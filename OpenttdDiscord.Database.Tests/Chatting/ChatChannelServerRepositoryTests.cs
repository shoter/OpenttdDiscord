﻿using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Testing;
using OpenttdDiscord.Testing.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Database.Tests.Chatting
{
    public class ChatChannelServerRepositoryTests : MysqlTest
    {
        public ChatChannelServerRepositoryTests() : base(new ContainerizedMysqlDatabase(), nameof(ChatChannelServerRepositoryTests))
        {
        }

        [Fact]
        public async Task AfterInsert_ShouldBeAbleToGetData()
        {
            var serverRepository = new ServerRepository(GetMysql());
            var chatRepository = new ChatChannelServerRepository(GetMysql());

            var server = await serverRepository.AddServer(11u, "127.0.0.1", 123, "test");
            await chatRepository.Insert(server, 133u);
            var response = await chatRepository.Get(server.Id, 133u);

            Assert.Equal("127.0.0.1", response.Server.ServerIp);
            Assert.Equal(123, response.Server.ServerPort);
            Assert.Equal("test", response.Server.ServerName);
            Assert.Equal(133u, response.ChannelId);
        }

        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenChatServerNotExist()
        {
            var chatRepository = new ChatChannelServerRepository(GetMysql());
            Assert.False(await chatRepository.Exists(12345u, 123123u));
        }

        [Fact]
        public async Task Exists_ShouldReturnTrue_WhenChatServerExist()
        {
            var serverRepository = new ServerRepository(GetMysql());
            var chatRepository = new ChatChannelServerRepository(GetMysql());

            var server = await serverRepository.AddServer(11u, "127.0.0.1", 123, "test");
            await chatRepository.Insert(server, 133u);

            Assert.True(await chatRepository.Exists(server.Id, 133u));
        }

        [Fact]
        public async Task Remove_ShouldRemoveServer()
        {
            var serverRepository = new ServerRepository(GetMysql());
            var chatRepository = new ChatChannelServerRepository(GetMysql());

            var server = await serverRepository.AddServer(11u, "127.0.0.1", 123, "test");
            await chatRepository.Insert(server, 133u);
            await chatRepository.Remove(server.Id, 133u);

            Assert.False(await chatRepository.Exists(server.Id, 133u));
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllServers()
        {
            var serverRepository = new ServerRepository(GetMysql());
            var chatRepository = new ChatChannelServerRepository(GetMysql());

            var toCreate = new ChatChannelServer[]
            {
                new ChatChannelServer()
                {
                    ChannelId = 123u,
                    Server = new Server(0, 11u, "127.0.0.1", 123, "test")
                },
                new ChatChannelServer()
                {
                    ChannelId = 122u,
                    Server = new Server(0, 17u, "127.0.0.1", 113, "abc")
                },
                new ChatChannelServer()
                {
                    ChannelId = 124u,
                    Server = new Server(0, 14u, "128.0.0.1", 143, "asdad")
                },           
            };

            foreach(var c in toCreate)
            {
                var server = await serverRepository.AddServer(c.Server.GuildId, c.Server.ServerIp, c.Server.ServerPort, c.Server.ServerName);
                var chatServer = await chatRepository.Insert(server, c.ChannelId);
            }

            var all = await chatRepository.GetAll();

            foreach(var a in all)
            {
                Assert.NotNull(toCreate.Single(x => x.ChannelId == a.ChannelId && x.Server.ServerIp == a.Server.ServerIp && x.Server.ServerPort == a.Server.ServerPort && x.Server.ServerName == a.Server.ServerName));
            }
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllServersForGivenGuild()
        {
            var serverRepository = new ServerRepository(GetMysql());
            var chatRepository = new ChatChannelServerRepository(GetMysql());

            var toCreate = new ChatChannelServer[]
            {
                new ChatChannelServer()
                {
                    ChannelId = 123u,
                    Server = new Server(0, 11u, "127.0.0.1", 123, "test")
                },
                new ChatChannelServer()
                {
                    ChannelId = 122u,
                    Server = new Server(0, 11u, "127.0.0.1", 113, "abc")
                },
                new ChatChannelServer()
                {
                    ChannelId = 124u,
                    Server = new Server(0, 11u, "128.0.0.1", 143, "asdad")
                },
            };

            var toCreateOther = new ChatChannelServer[]
            {
                new ChatChannelServer()
                {
                    ChannelId = 123u,
                    Server = new Server(0, 15u, "127.0.0.1", 123, "test")
                },
                new ChatChannelServer()
                {
                    ChannelId = 122u,
                    Server = new Server(0, 13u, "127.0.0.1", 113, "abc")
                },
                new ChatChannelServer()
                {
                    ChannelId = 124u,
                    Server = new Server(0, 1832u, "128.0.0.1", 143, "asdad")
                },
            };

            foreach (var c in toCreate)
            {
                var server = await serverRepository.AddServer(c.Server.GuildId, c.Server.ServerIp, c.Server.ServerPort, c.Server.ServerName);
                var chatServer = await chatRepository.Insert(server, c.ChannelId);
            }

            foreach (var c in toCreateOther)
            {
                var server = await serverRepository.AddServer(c.Server.GuildId, c.Server.ServerIp, c.Server.ServerPort, c.Server.ServerName);
                var chatServer = await chatRepository.Insert(server, c.ChannelId);
            }

            var all = await chatRepository.GetAll(11u);

            foreach (var a in all)
            {
                Assert.NotNull(toCreate.Single(x => x.Server.GuildId == 11u && x.ChannelId == a.ChannelId && x.Server.ServerIp == a.Server.ServerIp && x.Server.ServerPort == a.Server.ServerPort && x.Server.ServerName == a.Server.ServerName));
            }
        }



    }
}
