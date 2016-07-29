using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarAvoidable
{
    [TestClass]
    public class WhenTypeIsPlainClass : TestBase
    {
        private int _offset = 0;
        protected override int DiagnosticHorizontalOffset => _offset;

        [TestMethod]
        public void VariableInitializationWithPlainClassShouldWarn()
        {
            ShouldWarn("var x = Environment.OSVersion;", "OperatingSystem");
        }

        [TestMethod]
        public void ForeachOverPlainClassShouldWarn()
        {
            _offset = 9;
            ShouldWarn(
                "foreach (var x in System.Linq.Enumerable.Range(1, 10)) { Console.WriteLine(x); }",
                "int");
        }

        [TestMethod]
        public void UsingOverPlainClassShouldWarn()
        {
            _offset = 7;
            ShouldWarn(
                "using (var x = System.IO.File.Create(\"foo.bar\")) { x.WriteByte(42); }",
                "System.IO.FileStream");
        }
    }
}
