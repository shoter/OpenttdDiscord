﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using OpenTTDAdminPort;
using OpenTTDAdminPort.Events;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenttdDiscord.Backend.Admins;
using OpenttdDiscord.Common;
using OpenttdDiscord.Database.Chatting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenttdDiscord.Chatting
{
    public class ChatService : IChatService, IAdminPortClientUser
    {
        private readonly ILogger<ChatService> logger;
        private readonly IChatChannelServerService chatChannelServerService;
        private readonly IAdminPortClientProvider adminPortClientProvider;
        private readonly DiscordSocketClient discord;

        private readonly ConcurrentDictionary<(ulong, ulong), ChatChannelServer> chatServers = new ConcurrentDictionary<(ulong, ulong), ChatChannelServer>();
        private readonly ConcurrentQueue<ServerEvent> receivedMessagges = new ConcurrentQueue<ServerEvent>();
        private readonly ConcurrentQueue<DiscordMessage> discordMessages = new ConcurrentQueue<DiscordMessage>();
        private readonly ConcurrentQueue<ChatChannelServer> serversToRemove = new ConcurrentQueue<ChatChannelServer>();

        private readonly EmojiAsciiTranslator emojiTranslator = new EmojiAsciiTranslator();

        public ChatService(ILogger<ChatService> logger, IChatChannelServerService chatChannelServerService, IAdminPortClientProvider adminPortClientProvider, DiscordSocketClient discord)
        {
            this.logger = logger;
            this.chatChannelServerService = chatChannelServerService;
            this.adminPortClientProvider = adminPortClientProvider;
            this.discord = discord;
        }

        public async Task Start()
        {
            foreach (var s in await this.chatChannelServerService.GetAll())
                this.chatServers.TryAdd((s.Server.Id, s.ChannelId), s);

            this.chatChannelServerService.Added += (_, s) => chatServers.AddOrUpdate((s.Server.Id, s.ChannelId), s, (_, __) => s);
            this.chatChannelServerService.Removed += (_, s) => serversToRemove.Enqueue(s);

            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop()), null);
        }

        public async void MainLoop()
        {
            while (true)
            {
                try
                {
#if DEBUG
                    await Task.Delay(TimeSpan.FromSeconds(0.05));
#else
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
#endif

                    await RemoveServers();
                    await JoinUnjoinedClients();
                    await HandleGameMessages();
                    await HandleDiscordMessages();

                }
                catch (Exception e)
                {
                    logger.LogError($"Error {e.Message}", e);
                }
            }
        }

        private Task HandleDiscordMessages()
        {
            while (discordMessages.TryDequeue(out DiscordMessage msg))
            {
                string message = emojiTranslator.TranslateEmojisToAscii(msg.Message);
                var chatMsg = $"[Discord] {msg.Username}: {message}";

                IEnumerable<ChatChannelServer> others = chatServers.Values.Where(x => x.ChannelId == msg.ChannelId);
                foreach (var o in others)
                {
                    Server server = chatServers[(o.Server.Id, o.ChannelId)].Server;
                    IAdminPortClient client = adminPortClientProvider.GetClient(this, server);

                    if (client.ConnectionState == AdminConnectionState.Connected)
                    {
                        client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, 0, chatMsg));
                    }
                }
            }
            return Task.CompletedTask;
        }

        private async Task HandleGameMessages()
        {
            while (receivedMessagges.TryDequeue(out ServerEvent serverEvent))
            {
                if (!(serverEvent.AdminEvent is AdminChatMessageEvent msg))
                    continue;

                if (msg.Player.ClientId == 1)
                    continue;

                if ((msg.Message?.Trim() ?? string.Empty) == string.Empty)
                    continue;

                Server s = chatServers.Values.FirstOrDefault(c => c.Server.ServerIp == serverEvent.Server.ServerIp && c.Server.ServerPort == serverEvent.Server.ServerPort)?.Server;

                if (s == null)
                    continue;

                IEnumerable<ChatChannelServer> csList = chatServers.Values.Where(x => x.Server.Id == s.Id);

                foreach (var cs in csList)
                {
                    SocketTextChannel channel = null;
                    try
                    {
                        channel = discord.GetChannel(cs.ChannelId) as SocketTextChannel;

                        string message = emojiTranslator.TranslateAsciiToEmojis(msg.Message);
                        var chatMsg = $"[{cs.Server.ServerName}] {msg.Player.Name}: {message}";

                        await channel.SendMessageAsync(chatMsg);

                        chatMsg = $"[{cs.Server.ServerName}] {msg.Player.Name}: {msg.Message}";


                        IEnumerable<ChatChannelServer> others = chatServers.Values.Where(x => x.ChannelId == channel.Id && x.Server.Id != s.Id);
                        foreach (var o in others)
                        {
                            Server server = chatServers[(o.Server.Id, o.ChannelId)].Server;
                            IAdminPortClient client = adminPortClientProvider.GetClient(this, server);

                            if (client.ConnectionState == AdminConnectionState.Connected)
                            {
                                client.SendMessage(new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, 0, chatMsg));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError($"Could not send message to channel {channel?.Name ?? "null"} - {e.Message}");
                    }
                }
            }
        }

        private async Task JoinUnjoinedClients()
        {
            foreach (var s in chatServers.Values)
            {
                try
                {
                    if (adminPortClientProvider.IsRegistered(this, s.Server) == false)
                        await adminPortClientProvider.Register(this, s.Server);
                }
                catch (Exception e)
                {
                    logger.LogError($"{s.Server.ServerIp}:{s.Server.ServerPort.ToString()} - cannot join - {e.Message}");
                }
            }
        }

        private async Task RemoveServers()
        {
            while (serversToRemove.TryDequeue(out var s))
            {
                await adminPortClientProvider.Unregister(this, s.Server);
                chatServers.Remove((s.Server.Id, s.ChannelId), out _);
            }
        }

        public void AddMessage(DiscordMessage message)
        {
            this.discordMessages.Enqueue(message);
        }

        public void ParseServerEvent(Server server, IAdminEvent adminEvent)
        {
            this.receivedMessagges.Enqueue(new ServerEvent(server, adminEvent));
        }
    }
}
