using System;
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
        private static string LocalVarFixEquivalenceClassKey => FlyntaxAvoidVarAnalyzer.DiagnosticId + "LocalVar";
        private static string ForEachFixEquivalenceClassKey => FlyntaxAvoidVarAnalyzer.DiagnosticId + "ForEach";

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


            switch (context.Diagnostics.First().Properties["type"])
            {
                case FlyntaxAvoidVarAnalyzer.TargetIsLocalDeclaration:
                    RegisterCodeFix<LocalDeclarationStatementSyntax>(
                        context, root, ReplaceVarWithType, LocalVarFixEquivalenceClassKey);
                    break;

                case FlyntaxAvoidVarAnalyzer.TargetIsForEach:
                    RegisterCodeFix<ForEachStatementSyntax>(
                        context, root, ReplaceForeachVarWithType, ForEachFixEquivalenceClassKey);
                    break;
            }
        }

        private void RegisterCodeFix<TSyntax>(
            CodeFixContext context,
            SyntaxNode root,
            Func<Document, TSyntax, CancellationToken, Task<Document>> handler,
            string equivalenceClassKey)
        {
            // Find the local declaration statement identified by the diagnostic.
            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;
            TSyntax syntax = root.FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<TSyntax>()
                .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    "Replace var with type",
                    c => handler(context.Document, syntax, c),
                    equivalenceClassKey),
                diagnostic);
        }

        private async Task<Document> ReplaceForeachVarWithType(
            Document document,
            ForEachStatementSyntax foreachStatement,
            CancellationToken cancellationToken)
        {
            SemanticModel sm = await document.GetSemanticModelAsync(cancellationToken)
                .ConfigureAwait(false);
            var feinfo = sm.GetForEachStatementInfo(foreachStatement);
            ITypeSymbol variableTypeSymbol = feinfo.ElementType;

            string proposedTypeName = variableTypeSymbol.ToMinimalDisplayString(
                sm, foreachStatement.Type.SpanStart);

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);

            SyntaxNode newRoot = root.ReplaceNode(
                foreachStatement.Type,
                SyntaxFactory.ParseTypeName(proposedTypeName)
                    .WithLeadingTrivia(foreachStatement.Type.GetLeadingTrivia())
                    .WithTrailingTrivia(foreachStatement.Type.GetTrailingTrivia()));
            return document.WithSyntaxRoot(newRoot);
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