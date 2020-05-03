using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenttdDiscord.Admins;
using OpenttdDiscord.Chatting;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Openttd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class CommandHandlingService
    {
        private readonly CommandService commands;
        private readonly DiscordSocketClient discord;
        private readonly IChatService chatService;
        private readonly IServiceProvider services;
        private readonly IPrivateMessageHandlingService privateMessageService;
        private readonly IAdminService adminService;
        private readonly ILogger<CommandHandlingService> logger;


        public CommandHandlingService(IServiceProvider services, IPrivateMessageHandlingService privateMessageService, IAdminService adminService, CommandService commandService, DiscordSocketClient client, ILogger<CommandHandlingService> logger)
        {
            this.commands = commandService;
            this.privateMessageService = privateMessageService;
            this.discord = client;
            this.services = services;
            this.adminService = adminService;
            this.logger = logger;

            // Hook CommandExecuted to handle post-command-execution logic.
            commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            discord.MessageReceived += MessageReceivedAsync;

            this.chatService = services.GetRequiredService<IChatService>();
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            try
            {
                // Ignore system messages, or messages from other bots
                if (!(rawMessage is SocketUserMessage message)) return;
                if (message.Source != MessageSource.User) return;

                if (message.Channel is SocketDMChannel dmChannel)
                {
                    await this.privateMessageService.HandleMessage(message, dmChannel);
                    return;
                }

                // This value holds the offset where the prefix ends
                var argPos = 0;
                // Perform prefix check. You may want to replace this with
                // (!message.HasCharPrefix('!', ref argPos))
                // for a more traditional command format like !help.
                if (!message.HasMentionPrefix(discord.CurrentUser, ref argPos))
                {
                    this.chatService.AddMessage(new DiscordMessage()
                    {
                        ChannelId = message.Channel.Id,
                        Message = message.Content,
                        Username = message.Author.Username
                    });

                    // all messages needs to be routed to admin module. Maybe they contain commands - it will be evaluated by admin module.
                    await this.adminService.HandleMessage(message.Channel.Id, message.Content);

                    return;
                }

                this.logger.LogInformation($"Received command {message.Content} on {message.Channel.Name}");

                var context = new SocketCommandContext(discord, message);
                // Perform the execution of the command. In this method,
                // the command service will perform precondition and parsing check
                // then execute the command if one is matched.
                var result = await commands.ExecuteAsync(context, argPos, services);

                if(result.IsSuccess == false)
                {
                    logger.LogError($"Error executing {message.Content} on {message.Channel.Name} - {result.Error} - {result.ErrorReason}");
                }
                // Note that normally a result will be returned by this format, but here
                // we will handle the result in CommandExecutedAsync,
            }
            catch(Exception e)
            {
                logger.LogError($"{e.Message}");
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}
