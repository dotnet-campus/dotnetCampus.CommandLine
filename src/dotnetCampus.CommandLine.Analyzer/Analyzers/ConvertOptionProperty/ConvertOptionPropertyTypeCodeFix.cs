using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.CommandLine.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.CommandLine.Analyzers.ConvertOptionProperty
{
    public abstract class ConvertOptionPropertyTypeCodeFix : CodeFixProvider
    {
        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        protected abstract string CodeActionTitle { get; }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            if (root.FindNode(diagnosticSpan) is TypeSyntax typeSyntax)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: CodeActionTitle,
                        createChangedSolution: c => ConvertPropertyTypeAsync(context.Document, typeSyntax, c),
                        equivalenceKey: Resources.OptionLongNameMustBePascalCaseFix),
                    diagnostic);
            }
        }

        private async Task<Solution> ConvertPropertyTypeAsync(Document document, TypeSyntax typeSyntax, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document.Project.Solution;
            }

            var newRoot = root.ReplaceNode(typeSyntax, CreateTypeSyntaxNode(typeSyntax));
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

        protected abstract SyntaxNode CreateTypeSyntaxNode(TypeSyntax oldTypeSyntax);
    }
}
