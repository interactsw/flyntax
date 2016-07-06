using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseTypeHasAnonymousArgument : TestBase
    {
        [TestMethod]
        public void InitializerReturnsListOfAnonymousType()
        {
            ShouldNotWarn("var x = (new int[1]).Select(i => new { i }).ToList();");
        }
    }
}
