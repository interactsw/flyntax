using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests
{
    [TestClass]
    public class WhenTypeCannotBeInferred : TestBase
    {
        [TestMethod]
        public void DueToProblemsElsewhere()
        {
            ShouldNotWarn("Func<Spong> Foo = () => null; var x = Foo();");
        }
    }
}
