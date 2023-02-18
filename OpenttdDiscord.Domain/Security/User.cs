using Discord.WebSocket;

namespace OpenttdDiscord.Domain.Security;

public record User(string Name, UserLevel UserLevel)
{
    public User(SocketUser user)
        : this(user.Username, UserLevel.Admin)
    { }
    public override string ToString() => $"{Name}({UserLevel})";
}

