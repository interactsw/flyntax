using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAvoidable
{
    [TestClass]
    public class WhenTypeIsPlainClass : TestBase
    {
        [TestMethod]
        public void PlainClass()
        {
            ShouldWarn("var x = Environment.OSVersion;", "OperatingSystem");
        }
    }
}
