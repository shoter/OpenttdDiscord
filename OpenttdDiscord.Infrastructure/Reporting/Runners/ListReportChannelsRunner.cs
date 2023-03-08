using Discord;
using Discord.WebSocket;
using LanguageExt;
using OpenttdDiscord.Base.Basics;
using OpenttdDiscord.Base.Ext;
using OpenttdDiscord.Domain.Rcon.UseCases;
using OpenttdDiscord.Domain.Rcon;
using OpenttdDiscord.Domain.Security;
using OpenttdDiscord.Domain.Servers.UseCases;
using OpenttdDiscord.Infrastructure.Discord.Responses;
using OpenttdDiscord.Infrastructure.Discord.Runners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenttdDiscord.Domain.Reporting.UseCases;
using OpenttdDiscord.Domain.Reporting;

namespace OpenttdDiscord.Infrastructure.Reporting.Runners
{
    internal class ListReportChannelsRunner : OttdSlashCommandRunnerBase
    {
        private readonly DiscordSocketClient discord;

        private readonly IGetServerUseCase getServerUseCase;

        private readonly IListReportChannelsUseCase listReportChannelsUseCase;

        public ListReportChannelsRunner(
            DiscordSocketClient discord,
            IGetServerUseCase getServerUseCase,
            IListReportChannelsUseCase listReportChannelsUseCase)
        {
            this.discord = discord;
            this.getServerUseCase = getServerUseCase;
            this.listReportChannelsUseCase = listReportChannelsUseCase;
        }

        protected override EitherAsync<IError, ISlashCommandResponse> RunInternal(SocketSlashCommand command, User user, ExtDictionary<string, object> options)
        {
            ulong guildId = command.GuildId!.Value;

            return
                from reportServers in listReportChannelsUseCase.Execute(user, guildId)
                from response in GenerateResponse(reportServers)
                select (ISlashCommandResponse)new TextCommandResponse(response);
        }

        private EitherAsync<IError, string> GenerateResponse(List<ReportChannel> channels)
            => TryAsync(async () =>
            {
                StringBuilder sb = new();

                foreach (var reportChannel in channels)
                {
                    var server = (await getServerUseCase.Execute(User.Master, reportChannel.ServerId))
                    .ThrowIfError().Right();
                    sb.AppendLine($"{server.Name} - {MentionUtils.MentionChannel(reportChannel.ChannelId)}");
                }

                return sb.ToString();
            }).ToEitherAsyncError();
    }
}
