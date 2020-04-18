using RandomAnalyzers.RequiredMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Database
{
    public class MySqlConfig
    {
        [RequiredMember]
        public string ConnectionString { get; set; } 
    }
}
