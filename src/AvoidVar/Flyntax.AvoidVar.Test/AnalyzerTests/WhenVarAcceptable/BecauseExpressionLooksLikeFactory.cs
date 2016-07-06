using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAcceptable
{
    [TestClass]
    public class BecauseExpressionLooksLikeFactory : TestBase
    {
        [TestMethod]
        public void FactoryMethodDoesNotReportDiagnostic()
        {
            ShouldNotWarn(
                "var x = OtherType.Create();",
                OtherTypeWithMember("public static OtherType Create() => new OtherType();"));

        }
        [TestMethod]
        public void FullyQualifiedFactoryMethodDoesNotReportDiagnostic()
        {
            ShouldNotWarn(
                "var x = ConsoleApplication1.OtherType.Create();",
                OtherTypeWithMember("public static OtherType Create() => new OtherType();"));
        }


        [TestMethod]
        public void FactoryPropertyDoesNotReportDiagnostic()
        {
            ShouldNotWarn(
                "var x = OtherType.Instance;",
                OtherTypeWithMember("public static OtherType Instance { get; } = new OtherType();"));
        }

        [TestMethod]
        public void FullyQualifiedFactoryPropertyDoesNotReportDiagnostic()
        {
            ShouldNotWarn(
                "var x = ConsoleApplication1.OtherType.Instance;",
                OtherTypeWithMember("public static OtherType Instance { get; } = new OtherType();"));
        }
    }
}
