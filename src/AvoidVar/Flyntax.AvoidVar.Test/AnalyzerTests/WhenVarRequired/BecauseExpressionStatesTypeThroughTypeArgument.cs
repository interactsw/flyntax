using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseExpressionStatesTypeThroughTypeArgument : TestBase
    {
        [TestMethod]
        public void InitializerIsGenericMethodWithExplicitTypeArgument()
        {
            ShouldNotWarn("var x = (new int[1]).First<int>();");
        }
    }
}
