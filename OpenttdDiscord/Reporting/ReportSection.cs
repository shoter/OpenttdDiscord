using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Reporting
{
    public class ReportSection
    {
        public string Name { get; }

        public List<string> Data { get; } = new List<string>();

        public ReportSection(string name)
        {
            this.Name = name;
        }

        public void AddData(string data) => this.Data.Add(data);
        
    }
}
