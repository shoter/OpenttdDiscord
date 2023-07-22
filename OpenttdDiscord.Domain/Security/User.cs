using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Ext;

namespace OpenttdDiscord.Domain.Security;

public record User(
    string Name,
    UserLevel UserLevel)
{
    public User(SocketUser user)
        : this(
            user.Username,
            DetermineUserLevel(user))
    {
    }

    public User(
        SocketUser user,
        UserLevel userLevel)
        : this(
            user.Username,
            userLevel)
    {
    }

    public override string ToString() => $"{Name}({UserLevel})";

    public static User Master => new User(
        "Master",
        UserLevel.Admin);

    private static UserLevel DetermineUserLevel(SocketUser user)
    {
        if (user is SocketGuildUser guildUser)
        {
            if (guildUser.GuildPermissions.ManageGuild)
            {
                return UserLevel.Admin;
            }
        }

        return UserLevel.User;
    }

    public bool CheckIfHasCorrectUserLevel(UserLevel level)
    {
        return level switch
        {
            UserLevel.User => true,
            UserLevel.Moderator => UserLevel == UserLevel.Moderator || UserLevel == UserLevel.Admin,
            UserLevel.Admin => UserLevel == UserLevel.Admin,
            _ => false
        };
    }
}