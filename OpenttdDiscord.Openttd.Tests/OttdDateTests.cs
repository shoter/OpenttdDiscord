using OpenttdDiscord.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Openttd.Tests
{
    public class OttdDateTests
    {
        [InlineData(706891, 1935, 4, 28)]
        [Theory]
        public void DateCreation_ProperDateIsConstructed(uint tick, uint year, byte month, byte day)
        {
            OttdDate date = new OttdDate(tick);

            Assert.Equal(year, date.Year);
            Assert.Equal(month, date.Month);
            Assert.Equal(day, date.Day);
        }
        
    }
}
