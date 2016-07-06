using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.CodeFixTests
{
    [TestClass]
    public class WhenTypeIsPlainClass : TestBase
    {
        [TestMethod]
        public void PlainClass()
        {
            ShouldFix(
                "var x = Environment.OSVersion;",
                "OperatingSystem x = Environment.OSVersion;");
        }
    }
}
