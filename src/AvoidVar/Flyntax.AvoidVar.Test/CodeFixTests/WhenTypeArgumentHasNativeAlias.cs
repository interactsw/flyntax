using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.CodeFixTests
{
    [TestClass]
    public class WhenTypeArgumentHasNativeAlias : TestBase
    {
        [TestMethod]
        public void Int32()
        {
            ShouldFix(
                "var x = new int[0].ToList();",
                "List<int> x = new int[0].ToList();");
        }

        [TestMethod]
        public void Int32AndString()
        {
            ShouldFix(
                "var x = new int[0].ToDictionary(i => i.ToString());",
                "Dictionary<string, int> x = new int[0].ToDictionary(i => i.ToString());");
        }
    }
}
