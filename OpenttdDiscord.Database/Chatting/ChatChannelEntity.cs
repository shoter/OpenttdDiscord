﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using OpenttdDiscord.Domain.Chatting;

namespace OpenttdDiscord.Database.Chatting
{
    internal class ChatChannelEntity
    {
        public Guid ServerId { get; internal set; }

        public ulong GuildId { get; internal set; }

        public ulong ChannelId { get; internal set; }

        public ChatChannelEntity()
        {
        }

        public ChatChannelEntity(ChatChannel cc)
        {
            ServerId = cc.ServerId;
            GuildId = cc.GuildId;
            ChannelId = cc.ChannelId;
        }

        public ChatChannel ToDomain() => new(
            ServerId,
            GuildId,
            ChannelId);

        [ExcludeFromCodeCoverage]
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatChannelEntity>()
                .HasKey(x => new { x.ServerId, x.ChannelId });
        }
    }
}
