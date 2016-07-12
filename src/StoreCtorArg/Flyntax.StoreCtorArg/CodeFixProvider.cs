using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Flyntax.StoreCtorArg
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FlyntaxStoreCtorArgCodeFixProvider)), Shared]
    public class FlyntaxStoreCtorArgCodeFixProvider : CodeFixProvider
    {
        private const string title = "Store in field";
        private static string FixEquivalenceClassKey => FlyntaxStoreCtorArgAnalyzer.DiagnosticId;

        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
            ImmutableArray.Create(FlyntaxStoreCtorArgAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            ParameterSyntax parameter = root
                .FindToken(diagnosticSpan.Start)
                .Parent
                .AncestorsAndSelf()
                .OfType<ParameterSyntax>()
                .First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => StoreCtorArgInField(context.Document, parameter, c),
                    equivalenceKey: FixEquivalenceClassKey),
                diagnostic);
        }

        private async Task<Document> StoreCtorArgInField(
            Document document,
            ParameterSyntax parameter,
            CancellationToken cancellationToken)
        {
            ConstructorDeclarationSyntax ctorDecl = parameter.Parent.AncestorsAndSelf().OfType<ConstructorDeclarationSyntax>().First();
            TypeDeclarationSyntax typeDeclaration = ctorDecl.Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            SemanticModel sm = await document.GetSemanticModelAsync(cancellationToken);
            ISymbol paramTypeSymbol = sm.GetSymbolInfo(parameter.Type).Symbol;
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken)
                .ConfigureAwait(false);
            string fieldName = "_" + parameter.Identifier.Text;

            var existingField = typeDeclaration
                .Members
                .OfType<FieldDeclarationSyntax>()
                .FirstOrDefault(fd => fd.Declaration.Variables.Any(v => v.Identifier.Text == fieldName));
            if (existingField != null)
            {
                var si = sm.GetSymbolInfo(existingField.Declaration.Type);
                bool existingFieldHasDifferentType = si.Symbol != paramTypeSymbol;
                if (existingFieldHasDifferentType)
                {
                    // There's already a field Need to pick a different name
                    existingField = null;
                    for (int suffix = 1; true; ++suffix)
                    {
                        fieldName = "_" + parameter.Identifier.Text + suffix;
                        if (!typeDeclaration
                            .Members
                            .OfType<FieldDeclarationSyntax>()
                            .Any(fd => fd.Declaration.Variables.Any(v => v.Identifier.Text == fieldName)))
                        {
                            break;
                        }
                    }
                }
            }
            var fieldIdentifier = SyntaxFactory.Identifier(fieldName);
            var fieldAssignmentStatement = SyntaxFactory.SingletonList(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(fieldIdentifier),
                        SyntaxFactory.IdentifierName(parameter.Identifier))
                        ));
            var oldCtorBody = ctorDecl.Body;
            var newCtorBody = oldCtorBody.ChildNodes().Count() == 0
                ? SyntaxFactory.Block(fieldAssignmentStatement)
                : oldCtorBody.InsertNodesBefore(
                    oldCtorBody.ChildNodes().First(),
                    fieldAssignmentStatement);
            var newTypeDeclaration = typeDeclaration
                .ReplaceNode(oldCtorBody, newCtorBody);

            if (existingField == null)
            {
                var fieldVariableDeclaration = SyntaxFactory.VariableDeclaration(
                    parameter.Type,
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(fieldIdentifier)));
                var fieldDeclarationSyntax = SyntaxFactory.FieldDeclaration(
                            fieldVariableDeclaration)
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)))
                            .WithLeadingTrivia(SyntaxFactory.Whitespace("        "));

                newTypeDeclaration = newTypeDeclaration
                    .InsertNodesBefore(
                        FindNodeToInsertBefore(fieldName, newTypeDeclaration.Members),
                        // newTypeDeclaration.Members.First(),
                        SyntaxFactory.SingletonList(fieldDeclarationSyntax));
            }
            SyntaxNode newRoot = root.ReplaceNode(
                typeDeclaration,
                newTypeDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode FindNodeToInsertBefore(
            string identifierText,
            IEnumerable<SyntaxNode> items)
        {
            bool useNext = true;
            SyntaxNode result = null;
            foreach (SyntaxNode node in items)
            {
                if (useNext)
                {
                    result = node;
                    useNext = false;
                }

                var fieldDeclaration = node as FieldDeclarationSyntax;
                if (fieldDeclaration != null)
                {
                    if (fieldDeclaration.Modifiers.Any(m => m.Kind() == SyntaxKind.PrivateKeyword) &&
                        fieldDeclaration.Modifiers.Any(m => m.Kind() == SyntaxKind.ReadOnlyKeyword))
                    {
                        if (fieldDeclaration.Declaration.Variables.All(v =>
                            StringComparer.OrdinalIgnoreCase.Compare(v.Identifier.Text, identifierText) < 0))
                        {
                            useNext = true;
                        }
                    }
                }
            }

            return result;
        }
    }
}