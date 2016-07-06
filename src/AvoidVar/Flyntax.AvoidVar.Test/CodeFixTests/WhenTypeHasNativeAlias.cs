using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.CodeFixTests
{
    [TestClass]
    public class WhenTypeHasNativeAlias : TestBase
    {
        [TestMethod]
        public void Int32()
        {
            ShouldFix(
                "var x = Environment.TickCount;",
                "int x = Environment.TickCount;");
        }

        [TestMethod]
        public void Int64()
        {
            ShouldFix(
                "var x = new object[0].LongLength;",
                "long x = new object[0].LongLength;");
        }

        [TestMethod]
        public void String()
        {
            ShouldFix(
                "var x = Environment.CommandLine;",
                "string x = Environment.CommandLine;");
        }
    }
}
