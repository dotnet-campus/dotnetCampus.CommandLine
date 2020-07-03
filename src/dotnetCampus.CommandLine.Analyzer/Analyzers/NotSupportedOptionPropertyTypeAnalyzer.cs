using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
    public class NotSupportedOptionPropertyTypeAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Recognize these attributes.
        /// </summary>
        private readonly IList<string> _attributeNames = new List<string> { "Option", "OptionAttribute", "Value", "ValueAttribute" };

        private readonly IList<string> _nonGenericTypeNames = new List<string> { "String", "string", "Boolean", "bool", "Byte", "byte", "Short", "short", "Int32", "int", "Int64", "long", "Single", "float", "Double", "double", "Decimal", "decimal", "IList", "ICollection", "IEnumerable" };
        private readonly IList<string> _oneGenericTypeNames = new List<string> { "[]", "List", "IList", "IReadOnlyList", "Collection", "ICollection", "IReadOnlyCollection", "IEnumerable" };
        private readonly IList<string> _twoGenericTypeNames = new List<string> { "Dictionary", "IDictionary", "IReadOnlyDictionary", "KeyValuePair" };
        private readonly IList<string> _genericKeyArgumentTypeNames = new List<string> { "String", "string" };
        private readonly IList<string> _genericArgumentTypeNames = new List<string> { "String", "string" };

        /// <summary>
        /// Supported diagnostics.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticIds.NotSupportedOptionPropertyType,
            LocalizableStrings.Get(nameof(Resources.NotSupportedOptionPropertyTypeTitle)),
            LocalizableStrings.Get(nameof(Resources.NotSupportedOptionPropertyTypeMessage)),
            "Naming",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: LocalizableStrings.Get(nameof(Resources.NotSupportedOptionPropertyTypeDescription)),
            helpLinkUri: DiagnosticUrls.Get(DiagnosticIds.NotSupportedOptionPropertyType));

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
                    var (typeName, location) = AnalyzeOptionPropertyType(propertyNode);
                    if (typeName != null && location != null)
                    {
                        var diagnostic = Diagnostic.Create(Rule, location, typeName);
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Find LongName argument from the OptionAttribute.
        /// </summary>
        /// <param name="propertySyntax"></param>
        /// <returns>
        /// typeName: the LongName value.
        /// location: the syntax tree location of the LongName argument value.
        /// </returns>
        private (string? typeName, Location? location) AnalyzeOptionPropertyType(PropertyDeclarationSyntax propertySyntax)
        {
            var propertyTypeSyntax = propertySyntax.Type;
            string typeName = GetTypeName(propertyTypeSyntax);
            var (genericType0, genericType1) = GetGenericTypeNames(propertyTypeSyntax);

            if (typeName != null)
            {
                if (IsTwoGenericType(typeName)
                    && genericType0 != null && genericType1 != null
                    && IsGenericKeyArgumentType(genericType0)
                    && IsGenericArgumentType(genericType1))
                {
                        return (null, null);
                }
                else if (IsOneGenericType(typeName)
                    && genericType0 != null
                    && IsGenericArgumentType(genericType0))
                {
                    return (null, null);
                }
                else if (IsNonGenericType(typeName))
                {
                    return (null, null);
                }
            }

            return (propertyTypeSyntax.ToString(), propertyTypeSyntax.GetLocation());
        }

        private string GetTypeName(TypeSyntax typeSyntax)
        {
            if (typeSyntax is NullableTypeSyntax nullableTypeSyntax)
            {
                // string?
                typeSyntax = nullableTypeSyntax.ElementType;
            }

            if (typeSyntax is GenericNameSyntax genericNameSyntax)
            {
                // List<string>
                // Dictionary<string, string>
                return genericNameSyntax.Identifier.ToString();
            }
            else if (typeSyntax is ArrayTypeSyntax)
            {
                // string[]
                return "[]";
            }
            else if (typeSyntax is PredefinedTypeSyntax predefinedTypeSyntax)
            {
                // string
                return predefinedTypeSyntax.ToString();
            }
            else if (typeSyntax is QualifiedNameSyntax qualifiedNameSyntax)
            {
                // System.String
                return qualifiedNameSyntax.ChildNodes().OfType<IdentifierNameSyntax>().Last().ToString();
            }
            else
            {
                // String
                return typeSyntax.ToString();
            }
        }

        private (string?, string?) GetGenericTypeNames(TypeSyntax typeSyntax)
        {
            if (typeSyntax is NullableTypeSyntax nullableTypeSyntax)
            {
                // string?
                typeSyntax = nullableTypeSyntax.ElementType;
            }

            string? genericType0 = null, genericType1 = null;
            if (typeSyntax is GenericNameSyntax genericNameSyntax)
            {
                var genericTypes = genericNameSyntax.TypeArgumentList.ChildNodes().OfType<TypeSyntax>().ToList();
                genericType0 = GetTypeName(genericTypes[0]);
                if (genericTypes.Count == 2)
                {
                    genericType1 = GetTypeName(genericTypes[1]);
                }
                else if (genericTypes.Count > 2)
                {
                    genericType0 = null;
                    genericType1 = null;
                }
            }
            else if (typeSyntax is ArrayTypeSyntax arrayTypeSyntax)
            {
                genericType0 = GetTypeName(arrayTypeSyntax.ElementType);
            }
            return (genericType0, genericType1);
        }

        private bool IsNonGenericType(string typeName)
            => _nonGenericTypeNames.Contains(typeName, StringComparer.Ordinal);

        private bool IsOneGenericType(string typeName)
            => _oneGenericTypeNames.Contains(typeName, StringComparer.Ordinal);

        private bool IsTwoGenericType(string typeName)
            => _twoGenericTypeNames.Contains(typeName, StringComparer.Ordinal);

        private bool IsGenericKeyArgumentType(string typeName)
            => _genericKeyArgumentTypeNames.Contains(typeName, StringComparer.Ordinal);

        private bool IsGenericArgumentType(string typeName)
            => _genericArgumentTypeNames.Contains(typeName, StringComparer.Ordinal);
    }
}
