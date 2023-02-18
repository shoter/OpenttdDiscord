using Discord.WebSocket;

namespace OpenttdDiscord.Domain.Security;

public record User(string Name, UserLevel UserLevel)
{
    public User(SocketUser user)
        : this(user.Username, UserLevel.Admin)
    { }
    public override string ToString() => $"{Name}({UserLevel})";

    public static User Master => new User("Master", UserLevel.Admin);
}

