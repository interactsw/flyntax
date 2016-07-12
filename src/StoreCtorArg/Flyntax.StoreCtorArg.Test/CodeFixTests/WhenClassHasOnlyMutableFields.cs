using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.CodeFixTests
{
    [TestClass]
    public class WhenClassHasOnlyMutableFields : FixTestBase
    {
        protected override string Fields => @"        private string _aa;
        private string _bb;";
        protected override string NewFields => @"        private readonly string _arg;
        private string _aa;
        private string _bb;";
        protected override string NewBody => @"    _arg = arg;
        ";
    }
}
