using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test
{
    public abstract class ShouldNotWarnBase : TestBase
    {
        [TestMethod]
        public void ShouldNotProduceDiagnostic()
        {
            ShouldNotWarn();
        }
    }
}
