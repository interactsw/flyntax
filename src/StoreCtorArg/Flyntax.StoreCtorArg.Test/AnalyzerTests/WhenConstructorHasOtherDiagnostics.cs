using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests
{
    [TestClass]
    public class WhenConstructorHasOtherDiagnostics : TestBase
    {
        protected override string ArgumentList => "string arg,";

        [TestMethod]
        public void ShouldNotProduceDiagnostic()
        {
            ShouldNotWarn();
        }
    }
}
