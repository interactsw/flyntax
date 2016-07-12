using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.CodeFixTests
{
    public static class WhenClassHasReadOnlyFieldsInOrder
    {
        [TestClass]
        public class WhichAreAllAlphabeticallyBeforeAddedField : FixTestBase
        {
            protected override string Fields => @"        private readonly string _aa;
        private readonly string _aaa;
";
            protected override string NewFields => @"        private readonly string _aa;
        private readonly string _aaa;
        private readonly string _arg;
";
            protected override string NewBody => @"    _arg = arg;
        ";
        }
        [TestClass]
        public class WhichAreAllAlphabeticallyAfterAddedField : FixTestBase
        {
            protected override string Fields => @"        private readonly string _bb;
        private readonly string _bbb;
";
            protected override string NewFields => @"        private readonly string _arg;
        private readonly string _bb;
        private readonly string _bbb;
";
            protected override string NewBody => @"    _arg = arg;
        ";
        }
        [TestClass]
        public class WhereAddedFieldComesInBetweenExistingFieldsAlphabetically : FixTestBase
        {
            protected override string Fields => @"        private readonly string _aa;
        private readonly string _bb;
";
            protected override string NewFields => @"        private readonly string _aa;
        private readonly string _arg;
        private readonly string _bb;
";
            protected override string NewBody => @"    _arg = arg;
        ";
        }
    }
}
