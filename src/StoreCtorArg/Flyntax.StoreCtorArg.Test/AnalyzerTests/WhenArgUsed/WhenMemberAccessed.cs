using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests.WhenArgUsed
{
    public static class WhenMemberAccessed
    {
        [TestClass]
        public class Property : ShouldNotWarnBase
        {
            protected override string Body => "Console.WriteLine(arg.Length);";
        }

        [TestClass]
        public class Method : ShouldNotWarnBase
        {
            protected override string Body => "Console.WriteLine(arg.ToUpperInvariant());";
        }
    }
}
