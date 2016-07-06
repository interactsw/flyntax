using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseTypeIsAnonymous : TestBase
    {
        [TestMethod]
        public void ImmediateAnonymousType()
        {
            ShouldNotWarn("var x = new { x = 42, y = 99 };");
        }

        [TestMethod]
        public void ReturnedAnonymousType()
        {
            ShouldNotWarn("var x = new int[1].Select(i => new { x = 42, y = 99 }).Single();");
        }
    }
}
