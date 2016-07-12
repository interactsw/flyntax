using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.CodeFixTests
{
    [TestClass]
    public class WhenClassHasNoFields : FixTestBase
    {
        protected override string NewFields => @"        private readonly string _arg;
";
        protected override string NewBody => @"    _arg = arg;
        ";

    }
}
