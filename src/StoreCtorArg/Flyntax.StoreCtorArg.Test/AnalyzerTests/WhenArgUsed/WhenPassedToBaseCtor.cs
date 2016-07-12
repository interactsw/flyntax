using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests.WhenArgUsed
{
    [TestClass]
    public class WhenPassedToBaseCtor : ShouldNotWarnBase
    {
        protected override string BaseCall => " : base(arg)";
    }
}
