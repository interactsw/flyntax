using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Flyntax.AvoidVar
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FlyntaxAvoidVarAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AvoidVar";

        public const string TargetIsForEach = "ForEach";
        public const string TargetIsLocalDeclaration = "LocalDeclaration";
        public const string TargetIsUsingStatement = "UsingStatement";

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
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
            context.RegisterSyntaxNodeAction(OnLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
            context.RegisterSyntaxNodeAction(OnForEach, SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(OnUsingStatement, SyntaxKind.UsingStatement);
        }

        private void OnForEach(SyntaxNodeAnalysisContext context)
        {
            var statement = (ForEachStatementSyntax) context.Node;
            if (statement.ContainsDiagnostics)
            {
                // User probably still typing, so wait until it looks valid.
                return;
            }
            TypeSyntax variableManifestType = statement.Type;
            if (!statement.Type.IsVar)
            {
                return;
            }

            ForEachStatementInfo feinfo = context.SemanticModel.GetForEachStatementInfo(statement);
            ITypeSymbol variableTypeSymbol = feinfo.ElementType;
            if (variableTypeSymbol == null)
            {
                // We can't do anything if we don't know the type.
                // This typically happens when compilation errors prevent the compiler from
                // inferring the type. Although we bailed out earlier if there were diagnostics,
                // we can still get here because the problem might be elsehwhere. For example,
                // if the statement is "var x = Foo();" there won't be a diagnostic for that
                // statement, but if Foo() is incompletely defined, the type of 'x' may be
                // unavailable. (There will be a diagnostic in this case, but it will be attached
                // to the definition of Foo() and not to this local declaration.)
                return;
            }

            if (IsUnspeakable(variableTypeSymbol))
            {
                // Turns out var is necessary.
                return;
            }

            string proposedTypeName = variableTypeSymbol.ToMinimalDisplayString(
                context.SemanticModel, variableManifestType.SpanStart);
            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                variableManifestType.GetLocation(),
                ImmutableDictionary.Create<string, string>().Add("type", TargetIsForEach),
                proposedTypeName));
        }

        private void OnLocalDeclaration(SyntaxNodeAnalysisContext context)
        {
            var statement = (LocalDeclarationStatementSyntax) context.Node;
            if (statement.ContainsDiagnostics)
            {
                // User probably still typing, so wait until it looks valid.
                return;
            }

            VariableDeclarationSyntax declaration = statement.Declaration;
            TypeSyntax declarationManifestType = declaration.Type;
            if (!declarationManifestType.IsVar)
            {
                // We're only interested in kvetching about var so if it's not a
                // var declaration, return.
                return;
            }

            SymbolInfo symbolInfoForDeclarationType = context.SemanticModel.GetSymbolInfo(declarationManifestType);
            var declarationTypeSymbol = (ITypeSymbol) symbolInfoForDeclarationType.Symbol;
            if (declarationTypeSymbol == null)
            {
                // We can't do anything if we don't know the type.
                // This typically happens when compilation errors prevent the compiler from
                // inferring the type. Although we bailed out earlier if there were diagnostics,
                // we can still get here because the problem might be elsehwhere. For example,
                // if the statement is "var x = Foo();" there won't be a diagnostic for that
                // statement, but if Foo() is incompletely defined, the type of 'x' may be
                // unavailable. (There will be a diagnostic in this case, but it will be attached
                // to the definition of Foo() and not to this local declaration.)
                return;
            }

            if (IsUnspeakable(declarationTypeSymbol))
            {
                // Turns out var is necessary.
                return;
            }

            if (TypeIsExplicitInInitializer(declaration, declarationTypeSymbol, context))
            {
                return;
            }

            string proposedTypeName = symbolInfoForDeclarationType.Symbol.ToMinimalDisplayString(
                context.SemanticModel, declarationManifestType.SpanStart);
            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                declaration.Type.GetLocation(),
                ImmutableDictionary.Create<string, string>().Add("type", TargetIsLocalDeclaration),
                proposedTypeName));
        }

        private void OnUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var statement = (UsingStatementSyntax) context.Node;
            if (statement.ContainsDiagnostics)
            {
                // User probably still typing, so wait until it looks valid.
                return;
            }
            if (statement.Declaration == null)
            {
                // Must be of the form:
                //  using (something.OrOther())
                //  ...
                // (You're not required to have a var in there.)
                return;
            }

            TypeSyntax variableManifestType = statement.Declaration.Type;
            if (!variableManifestType.IsVar)
            {
                return;
            }

            SymbolInfo symbolInfoForVariableType = context.SemanticModel.GetSymbolInfo(variableManifestType);
            var variableTypeSymbol = (ITypeSymbol) symbolInfoForVariableType.Symbol;
            if (variableTypeSymbol == null)
            {
                // We can't do anything if we don't know the type.
                // This typically happens when compilation errors prevent the compiler from
                // inferring the type. Although we bailed out earlier if there were diagnostics,
                // we can still get here because the problem might be elsehwhere. For example,
                // if the statement is "var x = Foo();" there won't be a diagnostic for that
                // statement, but if Foo() is incompletely defined, the type of 'x' may be
                // unavailable. (There will be a diagnostic in this case, but it will be attached
                // to the definition of Foo() and not to this local declaration.)
                return;
            }

            if (IsUnspeakable(variableTypeSymbol))
            {
                // Turns out var is necessary.
                return;
            }

            string proposedTypeName = variableTypeSymbol.ToMinimalDisplayString(
                context.SemanticModel, variableManifestType.SpanStart);
            context.ReportDiagnostic(Diagnostic.Create(
                Rule,
                variableManifestType.GetLocation(),
                ImmutableDictionary.Create<string, string>().Add("type", TargetIsUsingStatement),
                proposedTypeName));
        }

        private static bool IsUnspeakable(ITypeSymbol symbol)
        {
            if (!symbol.CanBeReferencedByName)
            {
                return true;
            }

            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
            {
                return false;
            }

            return namedTypeSymbol.TypeArguments.Any(IsUnspeakable);
        }

        // Our goal is for the type of any variable to be immediately clear to anyone reading
        // the code, without having to perform type inference in their head. There are several
        // common uses of var in which the type is explicit in the initializer, e.g.:
        //  var x = (int) Foo();
        //  var y = new List<int>();
        //  var z = foo.Bar<int>();
        //  var q = MyType.DefaultInstance;
        //  var w = MyType.FromFoo(foo);
        // We don't want to force developers to repeat themselves thus:
        //  int x = (int) Foo();
        //  List<int> y = new List<int>();
        //  int z = foo.Bar<int>();
        //  MyType q = MyType.DefaultInstance;
        //  MyType w = MyType.FromFoo(foo);
        // So in these cases, we do not want to issue a diagnostic.
        private static bool TypeIsExplicitInInitializer(
            VariableDeclarationSyntax declaration,
            ITypeSymbol declarationTypeSymbol,
            SyntaxNodeAnalysisContext context)
        {
            ExpressionSyntax variableInitializer = declaration.Variables[0].Initializer.Value;
            if (variableInitializer is CastExpressionSyntax)
            {
                // Type is manifest because there's a cast, e.g.:
                //  var x = (int) Foo();
                return true;
            }
            if ((variableInitializer as BinaryExpressionSyntax)?.Kind() == SyntaxKind.AsExpression)
            {
                // Type is manifest because there's an 'as' expression e.g.:
                //  var x = Foo() as int;
                return true;
            }
            if (variableInitializer is ObjectCreationExpressionSyntax)
            {
                // Type is manifest because the expression invokes a constructor, e.g.
                //  var x = new List<int>();
                return true;
            }

            MemberAccessExpressionSyntax memberAccess = null;
            var invocation = variableInitializer as InvocationExpressionSyntax;
            if (invocation != null)
            {
                memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
                if (memberAccess != null)
                {
                    if (memberAccess.Name is GenericNameSyntax)
                    {
                        return true;
                    }
                }
            }
            else
            {
                memberAccess = variableInitializer as MemberAccessExpressionSyntax;
            }

            // If the expression is either of the form SomeType.Method() or
            // SomeType.Property, memberAccess will now be set (and will refer
            // to either SomeType.Method, or SomeType.Property respectively).
            if (memberAccess != null)
            {
                SymbolInfo symbolInfoForInitializer = context.SemanticModel.GetSymbolInfo(
                    memberAccess.Expression);
                var initializerTypeSymbol = symbolInfoForInitializer.Symbol as ITypeSymbol;
                if (initializerTypeSymbol == declarationTypeSymbol)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
