using Microsoft.VisualStudio.TestTools.UnitTesting;


[TestClass]
[DoNotParallelize] // Empêche uniquement cette classe d'être exécutée en parallèle
public class SampleTest
{
    [TestMethod]
    public void Test1()
    {
        Assert.IsTrue(true);
    }
}
