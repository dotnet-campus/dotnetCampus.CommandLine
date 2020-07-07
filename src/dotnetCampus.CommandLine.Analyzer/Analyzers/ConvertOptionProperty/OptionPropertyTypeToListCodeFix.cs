using System.Collections.Immutable;
using System.Composition;
using System.Threading;

using dotnetCampus.CommandLine.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.CommandLine.Analyzers.ConvertOptionProperty
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionPropertyTypeToListCodeFix)), Shared]
    public class OptionPropertyTypeToListCodeFix : ConvertOptionPropertyTypeCodeFix
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
            DiagnosticIds.SupportedOptionPropertyType,
            DiagnosticIds.NotSupportedOptionPropertyType);

        protected sealed override string CodeActionTitle => Resources.ConvertOptionPropertyTypeToListFix;

        protected sealed override SyntaxNode CreateTypeSyntaxNode(
            TypeSyntax oldTypeSyntax, CompilationUnitSyntax syntaxRoot, SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            //var oldUsing = syntaxRoot.Usings[1];
            //var newUsing = oldUsing.WithName(
            //    SyntaxFactory.QualifiedName(SyntaxFactory.QualifiedName(
            //        SyntaxFactory.IdentifierName("System"),
            //        SyntaxFactory.IdentifierName("Collections")),
            //        SyntaxFactory.IdentifierName("Generic")));

            //root = root.ReplaceNode(oldUsing, newUsing);

            return SyntaxFactory.ParseTypeName("System.Collections.Generic.List<string>");
        }
    }
}
