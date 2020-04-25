using OpenttdDiscord.Database.Chatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database.Servers
{
    public interface IServerService
    {
        event EventHandler<Server> Added;
        event EventHandler<NewServerPassword> NewServerPasswordRequestAdded;

        Task<Server> Getsert(ulong guildId, string ip, int port, string serverName);
        Task<bool> Exists(ulong guildId, string ip, int port);
        public Task<bool> Exists(ulong guildId, string serverName); 
        Task<Server> Get(ulong guildId, string ip, int port);

        Task<Server> Get(ulong guildId, string serverName);

        Task ChangePassword(ulong serverId, string newPassword);

        Task<List<Server>> GetAll(ulong guildId);

        void InformAboutNewPasswordRequest(NewServerPassword inRegister);
        NewServerPassword RemoveNewPasswordRequest(ulong userId);
        bool IsPasswordRequestInProgress(ulong userId);
        NewServerPassword GetNewPasswordProcess(ulong userId);
    }
}
