using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OpenttdDiscord.Chatting;
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

        public CommandHandlingService(IServiceProvider services, CommandService commandService, DiscordSocketClient client)
        {
            this.commands = commandService;
            this.discord = client;
            this.services = services;

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
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

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

                return;
            }

            var context = new SocketCommandContext(discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await commands.ExecuteAsync(context, argPos, services);
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
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
