﻿namespace OpenttdDiscord.Infrastructure.Chatting.Messages;

internal record UnregisterChatChannel(Guid ServerId, ulong GuildId, ulong ChannelId);