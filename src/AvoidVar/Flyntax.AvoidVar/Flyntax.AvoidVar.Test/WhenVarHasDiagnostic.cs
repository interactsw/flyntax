using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test
{
    // We ignore declarations which already have diagnostics, because
    // we can't really know what's going on (and certainly can't apply
    // a code fix) if the code does not make sense.
    [TestClass]
    public class WhenVarHasDiagnostic : TestBase
    {
        [TestMethod]
        public void WeDontAddOurOwnDiagnostic()
        {
            VerifyCSharpDiagnostic("var x =;");
        }
    }
}
