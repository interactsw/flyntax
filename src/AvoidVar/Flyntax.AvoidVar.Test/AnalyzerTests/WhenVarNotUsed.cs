using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests
{
    [TestClass]
    public class WhenVarNotUsed : TestBase
    {
        [TestMethod]
        public void NoDeclaration()
        {
            ShouldNotWarn("");
        }

        [TestMethod]
        public void TypedDeclaration()
        {
            ShouldNotWarn("int x = Environment.TickCount;");
        }

        [TestMethod]
        public void UsingWithoutDeclaration()
        {
            ShouldNotWarn("using (System.IO.File.Create(\"foo.bar\")) { }");
        }
    }
}
