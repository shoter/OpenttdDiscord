using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenttdDiscord.Database.Reporting;
using OpenttdDiscord.Database.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Commands
{
    public class ReportCommands : ModuleBase<SocketCommandContext>
    {
        public IReportServerService ReportService { get; set; }

        public IServerService ServerService { get; set; }

        [Command("register_report_channel")]
        [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RegisterChatServer(string serverName)
        {
            var server = await ServerService.Get(Context.Guild.Id, serverName);

            if (server == null)
            {
                await ReplyAsync("Server does not exist!");
                return;
            }

            var reportServer = await ReportService.Get(server.Id, Context.Channel.Id);

            if(reportServer != null)
            {
                await ReplyAsync("Report server is already registered here!");
                return;
            }

            await ReportService.Add(server, Context.Channel.Id);
            await ReplyAsync("Report server has been registered!");
        }
    }
}
