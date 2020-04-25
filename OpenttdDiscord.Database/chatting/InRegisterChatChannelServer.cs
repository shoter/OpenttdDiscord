using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Chatting
{
    public class NewServerPassword
    {
        public static TimeSpan DefaultExpiryTime = TimeSpan.FromMinutes(5);
        public ulong GuildId { get; set; }
        public string ServerName { get; set; }
        public ulong UserId { get; set; }
        public DateTime ExpiryTime { get; } = DateTime.Now.Add(DefaultExpiryTime);

    }
}
