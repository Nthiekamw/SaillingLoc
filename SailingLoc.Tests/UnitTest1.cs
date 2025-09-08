using Xunit;
using FluentAssertions;

namespace SailingLoc.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void SimpleTest()
        {
            // Exemple simple : addition
            (1 + 1).Should().Be(2);
        }
    }
}
