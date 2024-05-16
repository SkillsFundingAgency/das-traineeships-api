using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.FAT.Data.Extensions;
using System.Collections.Generic;

namespace SFA.DAS.FAT.Data.UnitTests.ExtensionTests
{
    public class WhenPopulatingParametersInAString
    {
        [Test]
        public void Then_Swaps_Each_Parameter()
        {
            //arr
            var source = "{one} {two} {three}";
            var parameters = new Dictionary<string, object>
            {
                {"one", 4},
                {"two", "stuff"},
                {"three", true}
            };
            var expected = "4 stuff True";

            //act
            var result = source.ReplaceParameters(parameters);

            //ass
            result.Should().Be(expected);
        }
    }
}