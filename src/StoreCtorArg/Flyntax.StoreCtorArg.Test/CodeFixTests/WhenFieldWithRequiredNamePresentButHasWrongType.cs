using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.CodeFixTests
{
    [TestClass]
    public class WhenFieldWithRequiredNamePresentButHasWrongType : FixTestBase
    {
        protected override string Fields => @"        private readonly int _arg;";

        protected override string NewBody => @"    _arg1 = arg;
        ";

        protected override string NewFields => @"        private readonly int _arg;
        private readonly string _arg1;
";
    }
}
