using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.CodeFixTests
{
    [TestClass]
    public class WhenTypeIsPlainClass : TestBase
    {
        [TestMethod]
        public void FixesVariableInitializationWithPlainClass()
        {
            ShouldFix(
                "var x = Environment.OSVersion;",
                "OperatingSystem x = Environment.OSVersion;");
        }

        [TestMethod]
        public void FixesForeachOverPlainClass()
        {
            ShouldFix(
                "foreach (var x in Enumerable.Range(1, 10).Select(i => Environment.OSVersion)) { Console.WriteLine(x); }",
                "foreach (OperatingSystem x in Enumerable.Range(1, 10).Select(i => Environment.OSVersion)) { Console.WriteLine(x); }");
        }

        [TestMethod]
        public void FixesUsingOverPlainClass()
        {
            ShouldFix(
                @"Func<Foo> foo = () => new Foo();
                  using (var f = foo()) {}",
                "class Foo : IDisposable { public void Dispose() {} }",
                @"Func<Foo> foo = () => new Foo();
                  using (Foo f = foo()) {}");

        }
    }
}
