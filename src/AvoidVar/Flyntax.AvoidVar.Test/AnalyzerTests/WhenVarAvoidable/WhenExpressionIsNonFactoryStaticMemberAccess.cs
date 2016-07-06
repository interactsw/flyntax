using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAvoidable
{
    [TestClass]
    public class WhenExpressionIsNonFactoryStaticMemberAccess : TestBase
    {
        [TestMethod]
        public void StaticMethodReportsDiagnostic()
        {
            ShouldWarn(
                "var x = OtherType.Foo();",
                OtherTypeWithMember("public static string Foo() => \"bar\";"),
                "string");
        }

        [TestMethod]
        public void StaticPropertyReportsDiagnostic()
        {
            ShouldWarn(
                "var x = OtherType.Instance;",
                OtherTypeWithMember("public static string Instance { get; } = \"x\";"),
                "string");
        }
    }
}
