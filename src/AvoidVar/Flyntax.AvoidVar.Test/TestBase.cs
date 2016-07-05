using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;

namespace Flyntax.AvoidVar.Test
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
        class TypeName
        {{
            private void Foo()
            {{
                {0}
            }}
        }}
    }}";

        protected static readonly DiagnosticResultLocation ResultLocation = new DiagnosticResultLocation("Test0.cs", 15, 17);

        protected static string WrapStatement(string statement) =>
            string.Format(Skeleton, statement);

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new FlyntaxAvoidVarCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new FlyntaxAvoidVarAnalyzer();
        }

        protected void ShouldNotWarn(string statement) => VerifyCSharpDiagnostic(WrapStatement(statement));

        protected void ShouldWarn(string statement, string expectedTypeName)
        {
            var expected = new DiagnosticResult
            {
                Id = "AvoidVar",
                Message = $"Use '{expectedTypeName}' instead of 'var'",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] { ResultLocation }
            };
            VerifyCSharpDiagnostic(WrapStatement(statement), expected);
        }

        protected void ShouldFix(string statement, string fixedStatement)
        {
            VerifyCSharpFix(WrapStatement(statement), WrapStatement(fixedStatement));
        }
    }
}
