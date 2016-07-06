using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseExpressionIsConstructor : TestBase
    {
        [TestMethod]
        public void TypedDeclaration()
        {
            ShouldNotWarn("var x = new List<int>();");
        }
    }
}
