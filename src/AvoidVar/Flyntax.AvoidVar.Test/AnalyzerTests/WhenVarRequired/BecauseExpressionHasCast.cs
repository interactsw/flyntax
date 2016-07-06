using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseExpressionHasCast : TestBase
    {
        [TestMethod]
        public void ClassicCast()
        {
            ShouldNotWarn("var x = (long) Environment.TickCount;");
        }

        [TestMethod]
        public void AsCast()
        {
            ShouldNotWarn("var x = \"\" as string;");
        }
    }
}
