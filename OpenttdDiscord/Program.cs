﻿using Discord;
using Discord.WebSocket;
using Discord.Commands;
using OpenttdDiscord.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using OpenttdDiscord.Database.Servers;
using OpenttdDiscord.Commands;
using Microsoft.Extensions.Configuration;
using OpenttdDiscord.Chatting;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Admins;
using OpenttdDiscord.Reporting;

namespace OpenttdDiscord
{
    internal class Program
    {
        private static DiscordSocketClient client;
        private static bool initialized = false;
        private static bool quitProgram = false;

        public static async Task Main()
        {
            var config = new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                   .Build();

            DependencyConfig.Init(config);

            using var services = DependencyConfig.ServiceProvider;

            client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += Log;
            services.GetRequiredService<CommandService>().Log += Log;

            await client.LoginAsync(TokenType.Bot, services.GetRequiredService<OpenttdDiscordConfig>().Token);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            client.Connected += Client_Connected;
            client.Disconnected += (_) => { quitProgram = true; return Task.CompletedTask; };


            // this freakin bots disconnects itself from time to time and it causes to use maximum CPU on raspberry pi where I am hosting my bot.
            // This causes to completely hang this bot and it's unresponsive as Discord Client will eat every possible CPU cycle to do ... nothing?
            // That's why I am quiting bot on first disconnection so it will automatically restart and not eat my cpu.
            // If there is a way to prevent that it would be super cool.
            // During 3rd of may testing this was happening around every hour.
            // An important note for sure is that it was not happening in the past (1-2 months ago).
            // Maybe the cause for this behaviour is that I still try to call discord api client when it is disconnected?
            // Rewriting parts of code to check if bot is connected or not and then send any messages would be helpfull but will it bring
            // better performance?
            while(quitProgram == false)
            {
                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }

        private static async Task Client_Connected()
        {
            if (initialized == false)
            {
                initialized = true;
                await DependencyConfig.ServiceProvider.GetRequiredService<ServerInfoProcessor>().Start();
                await DependencyConfig.ServiceProvider.GetRequiredService<IChatService>().Start();
                await DependencyConfig.ServiceProvider.GetRequiredService<IAdminService>().Start();
                await DependencyConfig.ServiceProvider.GetRequiredService<IReportService>().Start();
                DependencyConfig.ServiceProvider.GetRequiredService<IServerService>().NewServerPasswordRequestAdded += Program_NewServerPasswordRequestAdded; ;
            }
        }

        private static void Program_NewServerPasswordRequestAdded(object sender, NewServerPassword e)
        {
            client.GetUser(e.UserId).SendMessageAsync($"To complete registration of server please provide password to server {e.ServerName} in next message to this bot.");
        }

         private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
    }
}
