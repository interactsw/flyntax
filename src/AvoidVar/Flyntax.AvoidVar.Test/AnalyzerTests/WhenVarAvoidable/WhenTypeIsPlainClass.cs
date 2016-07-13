using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAvoidable
{
    [TestClass]
    public class WhenTypeIsPlainClass : TestBase
    {
        [TestMethod]
        public void VariableInitializationWithPlainClassShouldWarn()
        {
            ShouldWarn("var x = Environment.OSVersion;", "OperatingSystem");
        }

        [TestMethod]
        public void ForeachOverPlainClassShouldWarn()
        {
            ShouldWarn(
                "foreach (var x = Enumerable.Range(1, 10)) { Console.WriteLine(x); }",
                "int");
        }

    }
}
