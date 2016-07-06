using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAvoidable
{
    [TestClass]
    public class WhenTypeHasNativeAlias : TestBase
    {
        [TestMethod]
        public void Int32()
        {
            ShouldWarn("var x = Environment.TickCount;", "int");
        }

        [TestMethod]
        public void Int64()
        {
            ShouldWarn("var x = new object[0].LongLength;", "long");
        }

        [TestMethod]
        public void String()
        {
            ShouldWarn("var x = Environment.CommandLine;", "string");
        }
    }
}
