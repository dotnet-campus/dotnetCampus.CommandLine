using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using dotnetCampus.CommandLine.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.CommandLine.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OptionLongNameMustBePascalCaseAnalyzer : DiagnosticAnalyzer
    {
        private readonly IList<string> _attributeNames = new List<string> { "Option", "OptionAttribute" };

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticIds.OptionLongNameMustBePascalCase,
            LocalizableStrings.Get(nameof(Resources.OptionLongNameMustBePascalCaseTitle)),
            LocalizableStrings.Get(nameof(Resources.OptionLongNameMustBePascalCaseMessage)),
            "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: LocalizableStrings.Get(nameof(Resources.OptionLongNameMustBePascalCaseDescription)));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);

        private void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            var propertyNode = (PropertyDeclarationSyntax)context.Node;
            var list = propertyNode.AttributeLists.FirstOrDefault(x => x.Attributes.Any(y => y.Name.ToString() == "Option"));

            foreach (var attributeSyntax in propertyNode.AttributeLists.SelectMany(x => x.Attributes))
            {
                string? attributeName = attributeSyntax.Name switch
                {
                    IdentifierNameSyntax identifierName => identifierName.ToString(),
                    QualifiedNameSyntax qualifiedName => qualifiedName.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.ToString(),
                    _ => null,
                };

                if (attributeName != null && _attributeNames.Contains(attributeName))
                {
                    var (name, location) = AnalyzeOptionAttributeArguments(attributeSyntax);
                    if (name != null && location != null)
                    {
                        var diagnostic = Diagnostic.Create(Rule, location, name);
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
                }
            }
        }

        private (string? name, Location? location) AnalyzeOptionAttributeArguments(AttributeSyntax attributeSyntax)
        {
            var argumentList = attributeSyntax.ChildNodes().OfType<AttributeArgumentListSyntax>().FirstOrDefault();
            if (argumentList != null)
            {
                var expressions = argumentList.ChildNodes().OfType<AttributeArgumentSyntax>().Select(x => x.Expression);
                foreach (var expressionSyntax in expressions)
                {
                    var expression = expressionSyntax.ToString();
                    if (expression != null
                        && expression.StartsWith("\"", StringComparison.OrdinalIgnoreCase)
                        && expression.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
                    {
                        var value = expression.Substring(1, expression.Length - 2);
                        if (value.Length >= 2)
                        {
                            var isPascalCase = CheckIsPascalCase(value);
                            if (!isPascalCase)
                            {
                                return (value, expressionSyntax.GetLocation());
                            }
                        }
                        break;
                    }
                }
            }
            return (null, null);
        }

        private bool CheckIsPascalCase(string value)
        {
            var first = value[0];
            if (!char.IsUpper(first))
            {
                return false;
            }

            foreach (var letter in value)
            {
                if (!char.IsLetterOrDigit(letter))
                {
                    return false;
                }
            }

            if (value.Length >= 3)
            {
                var allUpper = value.All(x => char.IsUpper(x));
                if (allUpper)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
