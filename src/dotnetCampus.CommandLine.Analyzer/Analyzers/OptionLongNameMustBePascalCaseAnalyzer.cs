using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using dotnetCampus.Cli.Utils;
using dotnetCampus.CommandLine.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.CommandLine.Analyzers
{
    /// <summary>
    /// [Option("LongName")]
    /// The LongName must be PascalCase. If not, this analyzer report diagnostics.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OptionLongNameMustBePascalCaseAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Recognize these attributes.
        /// </summary>
        private readonly IList<string> _attributeNames = new List<string> { "Option", "OptionAttribute" };

        /// <summary>
        /// Supported diagnostics.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticIds.OptionLongNameMustBePascalCase,
            LocalizableStrings.Get(nameof(Resources.OptionLongNameMustBePascalCaseTitle)),
            LocalizableStrings.Get(nameof(Resources.OptionLongNameMustBePascalCaseMessage)),
            "dotnetCampus.Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: LocalizableStrings.Get(nameof(Resources.OptionLongNameMustBePascalCaseDescription)),
            helpLinkUri: DiagnosticUrls.Get(DiagnosticIds.OptionLongNameMustBePascalCase));

        /// <summary>
        /// Supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Register property analyzer.
        /// </summary>
        /// <param name="context"></param>
        public override void Initialize(AnalysisContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeProperty, SyntaxKind.PropertyDeclaration);
        }

        /// <summary>
        /// Find OptionAttribute from a property.
        /// </summary>
        /// <param name="context"></param>
        private void AnalyzeProperty(SyntaxNodeAnalysisContext context)
        {
            var propertyNode = (PropertyDeclarationSyntax)context.Node;

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

        /// <summary>
        /// Find LongName argument from the OptionAttribute.
        /// </summary>
        /// <param name="attributeSyntax"></param>
        /// <returns>
        /// name: the LongName value.
        /// location: the syntax tree location of the LongName argument value.
        /// </returns>
        private (string? name, Location? location) AnalyzeOptionAttributeArguments(AttributeSyntax attributeSyntax)
        {
            var argumentList = attributeSyntax.ChildNodes().OfType<AttributeArgumentListSyntax>().FirstOrDefault();
            if (argumentList != null)
            {
                var attributeArguments = argumentList.ChildNodes().OfType<AttributeArgumentSyntax>();
                foreach (var attributeArgument in attributeArguments)
                {
                    var expressionSyntax = attributeArgument.Expression;
                    var expression = expressionSyntax.ToString();
                    var nameEqualsExists = attributeArgument.ChildNodes().OfType<NameEqualsSyntax>().Any();
                    var longNameEqualsExists = attributeArgument.ChildNodes().OfType<NameEqualsSyntax>().Any(x => x.Name.ToString() == "LongName");
                    var mayBeLongName = !nameEqualsExists || longNameEqualsExists;
                    if (expression != null
                        && expression.StartsWith("\"", StringComparison.OrdinalIgnoreCase)
                        && expression.EndsWith("\"", StringComparison.OrdinalIgnoreCase)
                        && mayBeLongName)
                    {
                        var value = expression.Substring(1, expression.Length - 2);
                        if (value.Length >= 2)
                        {
                            var isPascalCase = NamingHelper.CheckIsPascalCase(value);
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
    }
}
