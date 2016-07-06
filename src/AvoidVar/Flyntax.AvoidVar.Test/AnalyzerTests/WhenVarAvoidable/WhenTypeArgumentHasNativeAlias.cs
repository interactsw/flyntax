using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAvoidable
{
    [TestClass]
    public class WhenTypeArgumentHasNativeAlias : TestBase
    {
        [TestMethod]
        public void Int32()
        {
            ShouldWarn("var x = new int[0].ToList();", "List<int>");
        }

        [TestMethod]
        public void Int32AndString()
        {
            ShouldWarn("var x = new int[0].ToDictionary(i => i.ToString());", "Dictionary<string, int>");
        }
    }
}
