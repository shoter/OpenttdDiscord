using OpenttdDiscord.Base.Basics;

namespace OpenttdDiscord.Infrastructure.Discord.CommandRunners
{
    public class OptionsDictionary : ExtDictionary<string, object>
    {
        public OptionsDictionary(ExtDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }
    }
}
