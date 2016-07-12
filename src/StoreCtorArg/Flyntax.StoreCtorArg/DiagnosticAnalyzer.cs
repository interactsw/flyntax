using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Flyntax.StoreCtorArg
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FlyntaxStoreCtorArgAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "StoreCtorArg";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }


        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(OnConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
        }

        private void OnConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConstructorDeclarationSyntax) context.Node;
            if (declaration.ContainsDiagnostics)
            {
                return;
            }

            foreach (ParameterSyntax arg in declaration.ParameterList.Parameters)
            {
                var argNameSymbol = new Lazy<ISymbol>(
                    () => context.SemanticModel.GetDeclaredSymbol(arg),
                    LazyThreadSafetyMode.None);
                var walker = new BodyWalker(context.SemanticModel, argNameSymbol);

                if (declaration.Initializer?.ArgumentList == null ||
                    !declaration.Initializer.ArgumentList.Arguments.Any(baseCallArg =>
                        context.SemanticModel.GetSymbolInfo(baseCallArg.Expression).Symbol?.OriginalDefinition == argNameSymbol.Value))
                {
                    declaration.Body.Accept(walker);
                    if (!walker.UsesArgument)
                    {
                        var diagnostic = Diagnostic.Create(Rule, arg.GetLocation(), arg.Identifier.Text);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private class BodyWalker : CSharpSyntaxWalker
        {
            private readonly SemanticModel _semanticModel;
            private readonly Lazy<ISymbol> _originalIdNameSymbolSource;

            public BodyWalker(
                SemanticModel semanticModel,
                Lazy<ISymbol> originalIdNameSymbolSource)
            {
                _semanticModel = semanticModel;
                _originalIdNameSymbolSource = originalIdNameSymbolSource;
            }

            public bool UsesArgument { get; private set; }

            private ISymbol OriginalIdNameSymbol => _originalIdNameSymbolSource.Value;

            public override void DefaultVisit(SyntaxNode node)
            {
                if (UsesArgument)
                {
                    return;
                }
                base.DefaultVisit(node);
            }

            // This catches not just plain assignment statements, but also assignments inside
            // object initializers, e.g. new Thing { Prop = arg }
            public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
            {
                var idName = node.Right as IdentifierNameSyntax;
                if (idName != null)
                {
                    SymbolInfo idNameSymbol = _semanticModel.GetSymbolInfo(idName);
                    if (idNameSymbol.Symbol?.OriginalDefinition == OriginalIdNameSymbol)
                    {
                        UsesArgument = true;
                        return;
                    }
                }
                base.VisitAssignmentExpression(node);
            }

            public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                SymbolInfo expressionSymbol = _semanticModel.GetSymbolInfo(node.Expression);
                var d = new Dictionary<string, string>();
                d["a"] = "b";
                if (expressionSymbol.Symbol?.OriginalDefinition == OriginalIdNameSymbol)
                {
                    UsesArgument = true;
                    return;
                }
                base.VisitMemberAccessExpression(node);
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (node.ArgumentList.Arguments.Any(arg =>
                    _semanticModel.GetSymbolInfo(arg.Expression).Symbol?.OriginalDefinition == OriginalIdNameSymbol))
                {
                    UsesArgument = true;
                    return;
                }
                base.VisitInvocationExpression(node);
            }

            public override void VisitInitializerExpression(InitializerExpressionSyntax node)
            {
                if (node.Expressions.OfType<IdentifierNameSyntax>().Any(idName =>
                    _semanticModel.GetSymbolInfo(idName).Symbol?.OriginalDefinition == OriginalIdNameSymbol))
                {
                    UsesArgument = true;
                    return;
                }
                base.VisitInitializerExpression(node);
            }

            public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
            {
                if (node.ArgumentList != null && node.ArgumentList.Arguments.Any(arg =>
                    _semanticModel.GetSymbolInfo(arg.Expression).Symbol?.OriginalDefinition == OriginalIdNameSymbol))
                {
                    UsesArgument = true;
                    return;
                }
                base.VisitObjectCreationExpression(node);
            }

            public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
            {
                if (node.ArgumentList.Arguments.Any(arg =>
                    _semanticModel.GetSymbolInfo(arg.Expression).Symbol?.OriginalDefinition == OriginalIdNameSymbol))
                {
                    UsesArgument = true;
                    return;
                }
                base.VisitElementAccessExpression(node);
            }
        }
    }
}
