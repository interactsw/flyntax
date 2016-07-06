using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests
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
            // The Console.WriteLine is there to use the variables, preventing the compiler from
            // adding its own diagnostic reporting that they are unused.
            ShouldNotWarn("int x = 42, y = 99; Console.WriteLine(x + y);");
        }
    }
}
