using Discord.WebSocket;

namespace OpenttdDiscord.Domain.Security;

public record User(string Name, UserLevel UserLevel)
{
    public User(SocketUser user)
        : this(user.Username, DetermineUserLevel(user))
    {
    }
    public override string ToString() => $"{Name}({UserLevel})";

    public static User Master => new User("Master", UserLevel.Admin);

    private static UserLevel DetermineUserLevel(SocketUser user)
    {
        if(user is SocketGuildUser guildUser)
        {
            if (guildUser.GuildPermissions.ManageGuild)
            {
                return UserLevel.Admin;
            }
        }

        return UserLevel.User;
    }
}

