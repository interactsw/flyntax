using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.CodeFixTests
{
    [TestClass]
    public class WhenTypeIsPlainClass : TestBase
    {
        [TestMethod]
        public void FixesVariableInitializationWithPlainClass()
        {
            ShouldFix(
                "var x = Environment.OSVersion;",
                "OperatingSystem x = Environment.OSVersion;");
        }

        [TestMethod]
        public void FixesForeachOverPlainClass()
        {
            ShouldFix(
                "foreach (var x = Enumerable.Range(1, 10)) { Console.WriteLine(x); }",
                "foreach (int x = Enumerable.Range(1, 10)) { Console.WriteLine(x); }");
        }
    }
}
