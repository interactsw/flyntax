using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.CodeFixTests
{
    [TestClass]
    public class WhenRequiredFieldPresentButNotAssignedFromCtor : FixTestBase
    {
        protected override string Fields => @"        private readonly string _arg;";

        protected override string NewBody => @"    _arg = arg;
        ";

        protected override string NewFields => Fields;
    }
}
