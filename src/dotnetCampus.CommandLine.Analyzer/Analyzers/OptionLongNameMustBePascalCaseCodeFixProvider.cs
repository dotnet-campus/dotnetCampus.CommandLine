using System;
using System.Collections.Generic;
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
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using dotnetCampus.CommandLine.Properties;
using System.Text;
using System.Globalization;

namespace dotnetCampus.CommandLine.Analyzers
{
    /// <summary>
    /// [Option("LongName")]
    /// The LongName must be PascalCase. If not, this codefix will fix it.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionLongNameMustBePascalCaseCodeFixProvider)), Shared]
    public class OptionLongNameMustBePascalCaseCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticIds.OptionLongNameMustBePascalCase);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return;
            }

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            ExpressionSyntax? syntax = root.FindNode(diagnosticSpan) switch
            {
                AttributeArgumentSyntax attributeArgumentSyntax => attributeArgumentSyntax.Expression,
                ExpressionSyntax expressionSyntax => expressionSyntax,
                _ => null,
            };

            if (syntax != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.OptionLongNameMustBePascalCaseFix,
                        createChangedSolution: c => MakePascalCaseAsync(context.Document, syntax, c),
                        equivalenceKey: Resources.OptionLongNameMustBePascalCaseFix),
                    diagnostic);
            }
        }

        private async Task<Solution> MakePascalCaseAsync(Document document, ExpressionSyntax expressionSyntax, CancellationToken cancellationToken)
        {
            var expression = expressionSyntax.ToString();
            var oldName = expression.Substring(1, expression.Length - 2);
            var newName = MakePascalCase(oldName);

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
            {
                return document.Project.Solution;
            }

            var newRoot = root.ReplaceNode(expressionSyntax,
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(newName)));
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

        private static string MakePascalCase(string oldName)
        {
            var builder = new StringBuilder();

            var isFirstLetter = true;
            var isWordStart = true;
            foreach (char c in oldName)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    // Append nothing because PascalCase has no special characters.
                    isWordStart = true;
                    continue;
                }

                if (isFirstLetter)
                {
                    if (char.IsDigit(c))
                    {
                        // PascalCase does not support digital as the first letter.
                        isWordStart = true;
                        continue;
                    }
                    else if (!char.IsUpper(c))
                    {
                        isFirstLetter = false;
                        isWordStart = false;
                        builder.Append(char.ToUpper(c, CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        isFirstLetter = false;
                        isWordStart = false;
                        builder.Append(c);
                    }
                }
                else
                {
                    if (char.IsDigit(c))
                    {
                        // PascalCase does not support digital as the first letter.
                        isWordStart = true;
                        builder.Append(c);
                    }
                    else if (!char.IsUpper(c))
                    {
                        builder.Append(isWordStart
                            ? char.ToUpper(c, CultureInfo.InvariantCulture)
                            : c);
                        isWordStart = false;
                    }
                    else
                    {
                        isWordStart = false;
                        builder.Append(c);
                    }
                }
            }

            return builder.ToString();
        }
    }
}
