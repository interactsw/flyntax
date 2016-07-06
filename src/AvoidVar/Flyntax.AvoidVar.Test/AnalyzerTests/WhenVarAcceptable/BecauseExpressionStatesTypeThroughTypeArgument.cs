using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAcceptable
{
    [TestClass]
    public class BecauseExpressionStatesTypeThroughTypeArgument : TestBase
    {
        [TestMethod]
        public void InitializerIsGenericMethodWithExplicitTypeArgumentDoesNotReportDiagnostic()
        {
            ShouldNotWarn("var x = (new int[1]).First<int>();");
        }
    }
}
