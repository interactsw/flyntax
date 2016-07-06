using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAcceptable
{
    [TestClass]
    public class BecauseExpressionIsConstructor : TestBase
    {
        [TestMethod]
        public void DoesNotReportDiagnostic()
        {
            ShouldNotWarn("var x = new List<int>();");
        }
    }
}
