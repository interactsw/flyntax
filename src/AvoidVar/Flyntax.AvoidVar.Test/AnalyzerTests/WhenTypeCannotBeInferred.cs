using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests
{
    [TestClass]
    public class WhenTypeCannotBeInferred : TestBase
    {
        [TestMethod]
        public void InLocalDeclarationDueToProblemsElsewhere()
        {
            ShouldNotWarn("Func<Spong> Foo = () => null; var x = Foo();");
        }

        [TestMethod]
        public void InForeachDueToProblemsElsewhere()
        {
            ShouldNotWarn("Func<IEnumerable<Spong>> Foo = () => null; foreach (var x in Foo()) { Console.WriteLine(x); }");
        }
    }
}
