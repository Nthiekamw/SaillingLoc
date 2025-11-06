using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaillingLoc.Helpers; // ton projet principal

namespace SailingLoc.Tests.Logic
{
    [TestClass]
    public class MathHelperTests
    {
        [TestMethod]
        public void Add_ShouldReturnCorrectSum()
        {
            var helper = new MathHelper();
            Assert.AreEqual(5, helper.Add(2, 3));
        }
    }
}
