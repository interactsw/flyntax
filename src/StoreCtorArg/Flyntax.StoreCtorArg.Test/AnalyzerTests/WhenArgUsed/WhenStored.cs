using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.AnalyzerTests.WhenArgUsed
{
    public static class WhenStored
    {
        [TestClass]
        public class InField : ShouldNotWarnBase
        {
            protected override string Fields => "private readonly string _value;";
            protected override string Body => "_value = arg;";
        }

        [TestClass]
        public class InProperty : ShouldNotWarnBase
        {
            protected override string Fields => "public string Value { get; }";
            protected override string Body => "Value = arg;";
        }
    }
}
