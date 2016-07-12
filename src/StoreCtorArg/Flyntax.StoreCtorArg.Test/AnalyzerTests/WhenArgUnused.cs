using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests
{
    public class WhenArgUnused : TestBase
    {
        [TestClass]
        public class SingleArgument : TestBase
        {
            [TestMethod]
            public void RaisesDiagnostic()
            {
                ShouldWarn();
            }
        }

        [TestClass]
        public class MultipleArgumentsSingleUnused : TestBase
        {
            protected override string Fields => "private readonly int _foo;";
            protected override string ArgumentList => "string arg, int foo";
            protected override string Body => "_foo = foo;";

            [TestMethod]
            public void RaisesDiagnostic()
            {
                ShouldWarn();
            }
        }
    }
}
