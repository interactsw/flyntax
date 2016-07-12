using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests.WhenArgUsed
{
    public static class WhenPassedAsArgument
    {
        public abstract class ToMethodBase : ShouldNotWarnBase
        {
            protected override string Fields => "private void UseIt(string value) => Console.WriteLine(value);";
        }

        [TestClass]
        public class ToMethodDirectly : ToMethodBase
        {
            protected override string Body => "UseIt(arg);";
        }

        [TestClass]
        public class ToMethodIndirectly : ToMethodBase
        {
            protected override string Body => "UseIt(\"somestring\".Replace(\"s\", arg));";
        }

        [TestClass]
        public class ToIndexer : ShouldNotWarnBase
        {
            protected override string Fields => "private readonly Dictionary<string, string> _d = new Dictionary<string, string>();";
            protected override string Body => "_d[arg] = \"foo\";";
        }

        [TestClass]
        public class ToConstructor : ShouldNotWarnBase
        {
            protected override string Fields => "private readonly KeyValuePair<string, string> _d;";
            protected override string Body => "_d = new KeyValuePair(arg, \"foo\")";
        }

        [TestClass]
        public class InSimpleCollectionInitializer : ShouldNotWarnBase
        {
            protected override string Fields => "private readonly List<string> _l;";
            protected override string Body => "_l = new Dictionary<string, string> { arg, \"foo\" };";
        }

        [TestClass]
        public class InStructuredCollectionInitializer : ShouldNotWarnBase
        {
            protected override string Fields => "private readonly Dictionary<string, string> _d;";
            protected override string Body => "_d = new Dictionary<string, string> { { arg, \"foo\" }, { \"x\", \"y\" } };";
        }

        [TestClass]
        public class InObjectInitializer : ShouldNotWarnBase
        {
            protected override string Fields => "private readonly UriBuilder _ub;";
            protected override string Body => "_ub = new UriBuilder { Host = arg };";
        }
    }
}
