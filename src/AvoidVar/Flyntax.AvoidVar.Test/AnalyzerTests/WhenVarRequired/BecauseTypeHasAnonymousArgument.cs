using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseTypeHasAnonymousArgument : TestBase
    {
        [TestMethod]
        public void InitializerReturningListOfAnonymousTypeShouldNotWarn()
        {
            ShouldNotWarn("var x = (new int[1]).Select(i => new { i }).ToList();");
        }

        [TestMethod]
        public void ForeachOverCollectionWithAnonymousElementTypeShouldNotWarn()
        {
            ShouldNotWarn("foreach (var x in (new int[1]).Select(i => new { i }) { Console.WriteLine(x.i); }");
        } 
    }
}
