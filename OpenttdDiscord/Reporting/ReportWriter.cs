using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Reporting
{
    public class ReportWriter
    {
        private readonly StreamWriter writer;
        public ReportWriter(StreamWriter streamWriter)
        {
            this.writer = streamWriter;
        }

        public async Task WriteReport(ReportMessage report)
        {
            await writer.WriteLineAsync($"Report creation date: {DateTimeOffset.Now:dd/MM/yyyy HH:mm zz}");
            await writer.WriteLineAsync($"Reporting player: {report.ReporterName}");
            await writer.WriteLineAsync($"Reason: {report.Reason}");
            await writer.WriteLineAsync(string.Empty);

            foreach (var s in report.Sections)
                await WriteSection(s);

            await writer.WriteLineAsync("Report end");
        }

        private async Task WriteSection(ReportSection section)
        {
            await writer.WriteLineAsync($"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            await writer.WriteLineAsync($"SECTION - {section.Name}");
            foreach (var d in section.Data)
                await writer.WriteLineAsync(d);
            await writer.WriteLineAsync($"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            await writer.WriteLineAsync(string.Empty);
        }

    }
}
