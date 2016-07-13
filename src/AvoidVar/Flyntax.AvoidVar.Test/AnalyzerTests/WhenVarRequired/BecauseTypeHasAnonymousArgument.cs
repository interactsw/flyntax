using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flyntax.AvoidVar.Test.AnalyzerTests.WhenVarRequired
{
    [TestClass]
    public class BecauseTypeHasAnonymousArgument : TestBase
    {
        [TestMethod]
        public void InitializerReturningListOfAnonymousTypeShouldNotWarn()
        {
            ShouldNotWarn("var x = (new int[1]).Select(i => new { i }).ToList();");
        }

        [TestMethod]
        public void ForeachOverCollectionOfCollectionsOfAnonymousTypesShouldNotWarn()
        {
            ShouldNotWarn("foreach (var x in new[] { (new int[1]).Select(i => new { i }) }) { Console.WriteLine(x[0].i); }");
        }

        [TestMethod]
        public void UsingOverClassWithAnonymousTypeArgShouldNotWarn()
        {
            ShouldNotWarn(
                "using (var x = Foo.Create(new { i }) { Console.WriteLine(\"foo\"); }",
                @"class Foo { public static Foo<T> Create<T>(T t) => new Foo<T>(); }
                  class Foo<T> : IDisposable
                  {
                      public void Dispose() {}
                  }");
        }
    }
}
