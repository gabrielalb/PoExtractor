using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace OrchardCoreContrib.PoExtractor.DotNet
{
    /// <summary>
    /// Extracts localizable string from data annotations error messages.
    /// </summary>
    public class ErrorMessageAnnotationStringExtractor : LocalizableStringExtractor<SyntaxNode>
    {
        private const string ErrorMessageAttributeName = "ErrorMessage";
        //private const string DisplayAttributeName = "Display";

        /// <summary>
        /// Creates a new instance of a <see cref="ErrorMessageAnnotationStringExtractor"/>.
        /// </summary>
        /// <param name="metadataProvider">The <see cref="IMetadataProvider{TNode}"/>.</param>
        public ErrorMessageAnnotationStringExtractor(IMetadataProvider<SyntaxNode> metadataProvider)
            : base(metadataProvider)
        {

        }

        /// <inheritdoc/>
        public override bool TryExtract(SyntaxNode node, out LocalizableStringOccurence result)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            result = null;

            if (node is AttributeArgumentSyntax argument
                && argument.Expression.Parent.ToFullString().StartsWith(ErrorMessageAttributeName)
                && node.Parent?.Parent is AttributeSyntax accessor
                //&& accessor.Name.ToString() == DisplayAttributeName
                && argument.Expression is LiteralExpressionSyntax literal
                && literal.IsKind(SyntaxKind.StringLiteralExpression))
            {
                result = CreateLocalizedString(literal.Token.ValueText, null, node);

                return true;
            }

            return false;
        }
    }
}
