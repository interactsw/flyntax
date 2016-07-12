using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests
{
    [TestClass]
    public class WhenNoArgs : TestBase
    {
        protected override string ArgumentList => "";

        [TestMethod]
        public void ShouldNotProduceDiagnostic()
        {
            ShouldNotWarn();
        }
    }
}
