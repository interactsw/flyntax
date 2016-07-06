using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Flyntax.AvoidVar
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FlyntaxAvoidVarCodeFixProvider)), Shared]
    public class FlyntaxAvoidVarCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(FlyntaxAvoidVarAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            // Find the local declaration statement identified by the diagnostic.
            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;
            LocalDeclarationStatementSyntax declaration = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<LocalDeclarationStatementSyntax>()
                .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Replace var with type",
                    c => ReplaceVarWithType(context.Document, declaration, c)),
                diagnostic);
        }

        private async Task<Document> ReplaceVarWithType(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel sm = await document.GetSemanticModelAsync(cancellationToken)
                .ConfigureAwait(false);

            TypeSyntax declarationManifestType = localDeclaration.Declaration.Type;
            SymbolInfo symbolInfoForDeclarationType = sm.GetSymbolInfo(declarationManifestType);
            var declarationTypeSymbol = (ITypeSymbol) symbolInfoForDeclarationType.Symbol;

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);
            SyntaxNode newRoot = root.ReplaceNode(
                declarationManifestType,
                SyntaxFactory.ParseTypeName(symbolInfoForDeclarationType.Symbol.ToMinimalDisplayString(sm, declarationManifestType.SpanStart))
                    .WithLeadingTrivia(declarationManifestType.GetLeadingTrivia())
                    .WithTrailingTrivia(declarationManifestType.GetTrailingTrivia()));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}