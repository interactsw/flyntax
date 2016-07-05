using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test
{
    // You can't actually declare multiple variables in a var, but we
    // need to make sure we don't crash when encountering explicitly-
    // typed multi-variable declarations.
    [TestClass]
    public class WhenDeclaringMultipleVariables : TestBase
    {
        [TestMethod]
        public void WeDontCrashOrAddDiagnostic()
        {
            // TODO: do we need to change this to ensure there isn't
            // an intrinsic diagnostic due to the variables never
            // being referenced?
            VerifyCSharpDiagnostic("int x = 42, y = 99;");
        }
    }
}
