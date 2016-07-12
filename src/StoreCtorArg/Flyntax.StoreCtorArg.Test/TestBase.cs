using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;

namespace Flyntax.StoreCtorArg.Test
{
    public abstract class TestBase : CodeFixVerifier
    {
        private const string Skeleton = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApplication1
{{
    class TypeName : Base
    {{
{0}
        public TypeName({1}) {2}
        {{
            {3}
        }}
    }}
    class Base
    {{
        private readonly string _value;
        public Base(string value) {{ _value = value; }}
        public Base() {{ }}
    }}
}}";

        protected static readonly DiagnosticResultLocation ResultLocation = new DiagnosticResultLocation("Test0.cs", 14, 25);

        protected virtual string Fields => "";
        protected virtual string ArgumentList => "string arg";
        protected virtual string BaseCall => "";
        protected virtual string Body => "";

        protected string Wrap() => 
            string.Format(Skeleton, Fields, ArgumentList, BaseCall, Body);

        protected void ShouldNotWarn() => VerifyCSharpDiagnostic(Wrap());

        protected void ShouldWarn()
        {
            var expected = new DiagnosticResult
            {
                Id = "StoreCtorArg",
                Message = $"Store constructor argument 'arg' in a field",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { ResultLocation }
            };
            VerifyCSharpDiagnostic(Wrap(), expected);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider() =>
            new FlyntaxStoreCtorArgCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new FlyntaxStoreCtorArgAnalyzer();
    }
}
