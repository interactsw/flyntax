using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.StoreCtorArg.Test.CodeFixTests
{
    public abstract class FixTestBase : TestBase
    {
        protected abstract string NewFields { get; }
        protected abstract string NewBody { get; }

        [TestMethod]
        public void ShouldFix()
        {
            VerifyCSharpFix(
                Wrap(),
                string.Format(Skeleton, NewFields, ArgumentList, BaseCall, NewBody));
        }
    }
}
