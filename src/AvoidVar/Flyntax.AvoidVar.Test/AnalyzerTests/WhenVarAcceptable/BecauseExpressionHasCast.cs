using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAcceptable
{
    [TestClass]
    public class BecauseExpressionHasCast : TestBase
    {
        [TestMethod]
        public void ClassicCastDoesNotReportDiagnostic()
        {
            ShouldNotWarn("var x = (long) Environment.TickCount;");
        }

        [TestMethod]
        public void AsCastDoesNotReportDiagnostic()
        {
            ShouldNotWarn("var x = \"\" as string;");
        }
    }
}
