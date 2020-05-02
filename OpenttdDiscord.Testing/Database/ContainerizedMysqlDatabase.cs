using Docker.DotNet;
using Docker.DotNet.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenttdDiscord.Common;

namespace OpenttdDiscord.Testing.Database
{
    public class ContainerizedMysqlDatabase : IContainerizedMysqlDatabase, IDisposable
    {
        public int Port { get; private set; } = 0;

        private string containerName;

        private readonly DockerClient client;

        public ContainerizedMysqlDatabase()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                client = new DockerClientConfiguration(
                new Uri("tcp://docker:2376"))
                .CreateClient();

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                client = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine"))
                .CreateClient();
            }
            else
            {
                throw new NotSupportedException("This os is not supported");
            }

        }

        public void Dispose()
        {
            if (client != null)
            {
                client.Containers.StopContainerAsync(containerName, new ContainerStopParameters()).Wait();
                client.Containers.RemoveContainerAsync(containerName,
                    new ContainerRemoveParameters { Force = true, RemoveVolumes = true }).Wait();
                client.Dispose();
            }
        }

        public async Task Start(string containerName)
        {
            // https://github.com/dotnet/Docker.DotNet/issues/402 - There is some nice info in this issue with nice real life example.
            this.containerName = containerName;

            await RemoveContainerIfExists(containerName);
            await StartContainer(containerName);
            await WaitForDatabaseToStart();
        }

        private async Task StartContainer(string containerName)
        {
            var response = await client.Containers.CreateContainerAsync(new Docker.DotNet.Models.CreateContainerParameters()
            {
                Name = containerName,
                Image = "openttd-discord-mysql",
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "3306/tcp",
                            new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = Port.ToString()
                                }
                            }
                        }
                    },
                },

                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    {
                        "3306/tcp", new EmptyStruct(){ }
                    }
                }
              });

            await client.Networks.ConnectNetworkAsync("jenkins", new NetworkConnectParameters
            {
                Container = response.ID,
            });

            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters() { });
        }

        private async Task RemoveContainerIfExists(string containerName)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters() { Limit = 1000 });
            string containerId = null;

            this.Port = GetFreeTcpPort();

            foreach (var c in containers)
            {
                foreach (var name in c.Names)
                    if (name.TrimStart('/') == containerName)
                    {
                        containerId = c.ID;
                        break;
                    }
                if (containerId != null)
                    break;
            }

            if (containerId != null)
            {
                await client.Containers.StopContainerAsync(containerId, new ContainerStopParameters() { });
                await client.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters() { Force = true });
            }
        }

        private async Task InitialiseSchema()
        {
            string workingDirectory = Environment.CurrentDirectory;
            var dir = Path.Combine(Directory.GetParent(workingDirectory).Parent.Parent.Parent.FullName, "OpenttdDiscord.Database/SQL");
            var files = Directory.GetFiles(dir);

            using var conn = new MySqlConnection(GetConnectionString());
            await conn.OpenAsync();

            foreach (var f in files.OrderBy(x => x))
            {
                var path = Path.Combine(dir, f);
                string sql = File.ReadAllText(path);

                using var cmd = new MySqlCommand(sql, conn);

                await cmd.ExecuteNonQueryAsync();

            }
        }

        private async Task WaitForDatabaseToStart()
        {
            DateTime waitingTimeEnd = DateTime.Now.AddMinutes(2);
            while (DateTime.Now < waitingTimeEnd)
            {
                try
                {
                    using var conn = new MySqlConnection(GetConnectionString());
                    await conn.OpenAsync();
                    using var cmd = new MySqlCommand("select 1", conn);

                    await cmd.ExecuteNonQueryAsync();
                    break;
                }
                catch (MySqlException e)
                {
                    if (e.ErrorCode != 1042 && e.Message != "Couldn't connect to server" && !e.Message.Contains("Unable to connect to any of the specified MySQL hosts"))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/138043/find-the-next-tcp-port-in-net
        /// </summary>
        /// <returns></returns>
        private static int GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public string GetConnectionString() => $"Server=127.0.0.1;Port={Port};Database=openttd;Uid=openttd;Pwd=test";
    }
}
