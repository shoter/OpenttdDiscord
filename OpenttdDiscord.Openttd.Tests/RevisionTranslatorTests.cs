using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenttdDiscord.Openttd.Tests
{
    public class RevisionTranslatorTests
    {
        private readonly RevisionTranslator translator = new RevisionTranslator();

        [Theory]
        [InlineData("1.10.0-RC1", 1, 10, 0, 0)]
        [InlineData("1.9.3", 1, 9, 3, 1)]
        public void TranslateToNewgrfVersion_ShouldCorrectlyTranslate(string revision, int expectedMajor, int expectedMinor, int expectedBuild, int expectedRelease)
        {
            var newgrfVersion = translator.TranslateToNewGrfRevision(revision);

            Assert.Equal(expectedMajor, newgrfVersion.Major);
            Assert.Equal(expectedMinor, newgrfVersion.Minor);
            Assert.Equal(expectedBuild, newgrfVersion.Build);
            Assert.Equal(expectedRelease, newgrfVersion.Release);
        }


    }
}
