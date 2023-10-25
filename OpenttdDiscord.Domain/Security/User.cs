using Discord;

namespace OpenttdDiscord.Domain.Security;

public record User(
    string Name,
    UserLevel UserLevel)
{
    public User(IUser user)
        : this(
            user.Username,
            DetermineUserLevel(user))
    {
    }

    public User(
        IUser user,
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

    private static UserLevel DetermineUserLevel(IUser user)
    {
        if (user is IGuildUser guildUser)
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